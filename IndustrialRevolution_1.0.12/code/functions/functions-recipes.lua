------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-log")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- create a new generated basic item recipe

function generate_recipe(name, result, icon_name, material, component, category, order, result_count, ingredients, energy, enabled, icon_name, subgroup, emissions)
    local recipe = {}
    recipe.type = "recipe"
    recipe.name = name
	local materialdata = DIR.materials[material]
	local componentdata = DIR.components[component]
	local main_ingredient = ingredients[1].name
	if not main_ingredient then error("Missing ingredient name in "..name) end
	recipe.results = { { name = result, type = (component == "fluid") and "fluid" or "item", amount = result_count } }
	if componentdata.category_results_secondary and componentdata.category_results_secondary[category] then
		table.insert(recipe.results, componentdata.category_results_secondary[category])
	end
	recipe.show_amount_in_title = false
	recipe.always_show_products = (result_count > 1)
	recipe.subgroup = subgroup
    recipe.enabled = enabled
    recipe.category = category
    recipe.order = order
    recipe.allow_decomposition = not list_contains(DIR.non_decomposition_components, component)
    recipe.ingredients = ingredients
    recipe.energy_required = energy
	recipe.icon = get_item_icon(name, icon_name)
	recipe.icon_size = DIR.icon_size
	-- locale
	if componentdata.recipe_locale_name_exceptions and componentdata.recipe_locale_name_exceptions[material] then
		recipe.localised_name = {componentdata.recipe_locale_name_exceptions[material]}
	elseif string.find(category, "smelting") then
		if DIR.materials[material].metal or (material == "glass") then
			if DIR.materials[material].alloy and string.find(name,"ingot%-from%-ingot") then
				recipe.localised_name = {"recipe-name.alloying", {"item-name."..result}}
			else
				recipe.localised_name = {"recipe-name.smelting", {"item-name."..main_ingredient}}
			end
		end
	elseif string.find(category, "washing") then
		recipe.localised_name = {"recipe-name."..category, {"item-name."..material}}
	elseif string.find(category, "chemistry") then
		recipe.localised_name = {"recipe-name.chemistry", {"item-name."..result}}
	else
		recipe.localised_name = {"item-name."..result}
	end
    -- tints
	if materialdata.hue then
		recipe.crafting_machine_tint = get_crafting_tints_from_hue(materialdata.hue,materialdata.saturation)
		if materialdata.metal_colour then recipe.crafting_machine_tint.tertiary = materialdata.metal_colour end
	end
	-- pollution bonus/penalty
	if emissions then recipe.emissions_multiplier = emissions end
    data:extend({recipe})
    return data.raw.recipe[name]
end

-- gravels and pures get probability byproducts

function add_probability_to_recipe(recipe, item, component, bonus_results, generate_multi_icons, walking, result_probability, byproduct_probability_multiplier)
	if not byproduct_probability_multiplier then byproduct_probability_multiplier = 1 end
	if not result_probability then result_probability = 1 end
	local results = table.deepcopy(recipe.results)
	-- if item.localised_description then 
		-- recipe.localised_description = {"", item.localised_description, "\n", {"recipe-description.bonus"} }
	-- else
		-- recipe.localised_description = {"", {"recipe-description.bonus"} }
	-- end
	for k,v in pairs(results) do
		if v.type == "item" then
			if result_probability < 1 then results[k].probability = result_probability end
			-- table.insert(recipe.localised_description, "\n")
			-- table.insert(recipe.localised_description, "[img=item/"..results[k].name.."] [font=default][color=1,0.74,0.4]")
			-- table.insert(recipe.localised_description, {"item-name."..results[k].name})
			-- table.insert(recipe.localised_description, " ("..(math.floor(result_probability*10000)/100).."%)[/color][/font]")
		end
	end
	for bonus_item,probability in pairs(bonus_results) do
		table.insert(results, { name = bonus_item, type = "item", amount = 1, probability = probability * byproduct_probability_multiplier })
		-- table.insert(recipe.localised_description, "\n")
		-- table.insert(recipe.localised_description, "[img=item/"..bonus_item.."] [font=default][color=1,0.74,0.4]")
		-- table.insert(recipe.localised_description, {"item-name."..bonus_item})
		-- table.insert(recipe.localised_description, " ("..(math.floor(probability * byproduct_probability_multiplier * 10000)/100).."%)[/color][/font]")
	end
	-- we just added a recipe description which screws up item descriptions good and proper (0.17.68), so, fix it for our use cases only
	-- if item.fuel_value then
		-- local fuel = format_number_to_units(util.parse_energy(item.fuel_value),true).."J"
		-- table.insert(recipe.localised_description, "\n")
		-- table.insert(recipe.localised_description, {"recipe-description.material-fuel-value", fuel})
	-- end
	-- if item.fuel_emissions_multiplier then
		-- local pollution = string.format("%d%%", item.fuel_emissions_multiplier*100)
		-- table.insert(recipe.localised_description, "\n")
		-- table.insert(recipe.localised_description, {"recipe-description.material-pollution", pollution})
	-- end
	-- if walking then
		-- table.insert(recipe.localised_description, "\n")
		-- table.insert(recipe.localised_description, {"recipe-description.component-is-a-tile", math.floor(walking*100)})	
		-- table.insert(recipe.localised_description, "\n")
		-- table.insert(recipe.localised_description, {"recipe-description.font-wrap-open"})		
		-- table.insert(recipe.localised_description, {"description.used-to-mine-paths"})		
		-- table.insert(recipe.localised_description, {"recipe-description.font-wrap-close"})		
	-- end
	recipe.results = results
	recipe.result = nil
	recipe.result_count = nil
	recipe.main_product = item.name
	if generate_multi_icons then
		add_result_icons_to_recipe(recipe, "dirty-water")
	else
		recipe.icon = data.raw.item[results[1].name].icon
		recipe.icon_size = data.raw.item[results[1].name].icon_size
	end
end

function is_an_ignored_recipe(name)
	local recipe = data.raw.recipe[name]
	if recipe and recipe.IR_tech_rebuild_ignore == true then return true end
	for _,ignore in pairs(DIR.redundant_recipe_exclusions) do
		if string.find(name,ignore) then return true end
	end
	return false
end

function is_an_ignored_technology(name)
	local tech = data.raw.technology[name]
	if tech and tech.IR_tech_rebuild_ignore == true then return true end
	return is_value_in_table(DIR.ancestor_tech_ignore_list, name)
end

function any_recipe_uses_ingredient(target)
	for _,recipe in pairs(data.raw.recipe) do
		local root = recipe.ingredients or (recipe.normal and recipe.normal.ingredients)
		if root and not recipe.redundant and not is_an_ignored_recipe(recipe.name) then
			for _,ingredient in pairs(root) do
				local name = ingredient.name or ingredient[1]
				if name == target then return true end
			end
		end
	end
	return false
end

-- scan all recipes for items which are never used, recursively

function redundancy_scan()
	local items = {}
	for component,componentdata in pairs(DIR.components) do
		if componentdata.type == "item" and componentdata.materials and not componentdata.skip_scan then
			for _,material in pairs(componentdata.materials) do
				items[get_item_name(material,component)] = true
			end
		end
	end
	for count = 1, 100 do
		local found = false
		spam_log("Redundancy scan, pass "..count.." ...")
		for target,_ in pairs(items) do
			if not any_recipe_uses_ingredient(target) then
				found = true
				spam_log("Redundant item: "..target)
				items[target] = nil
				local recipe = data.raw.recipe[target]
				if recipe then 
					recipe.redundant = true
					if recipe.icon then
						recipe.icons = { { icon = recipe.icon, icon_size = recipe.icon_size } }
						recipe.icon = nil
						recipe.icon_size = nil
					end
					table.insert(recipe.icons, { icon = get_icon_path("signal_warning",64), icon_size = 64, scale = 0.25, shift = {-8,8} })
					recipe.localised_description = {"recipe-description.redundant"}
					if settings.startup["deadlock-industry-hide-redundancies"].value then
						recipe.hidden = true
						local item = data.raw.item[target]
						item.flags = {"hidden"}
					end
				end
			end
		end
		if not found then 
			spam_log("Redundancy scan complete.")
			return
		end
	end
end

-- search through a set of ingredients and replace matches, returns true if any replacements made
function replace_ingredients(ingredients, from, to)
    local found = false
    for _,ingredient in pairs(ingredients) do
        if ingredient.name == from then
            ingredient.name = to
            found = true
        elseif ingredient[1] == from then
            ingredient[1] = to
            found = true
        end
    end
    return found
end

-- replace any target ingredients in all recipes
function replace_all_ingredients(from, to)
    for _,recipe in pairs(data.raw.recipe) do
        local found = false
        if recipe.ingredients then found = replace_ingredients(recipe.ingredients, from, to) end
        if recipe.normal and recipe.normal.ingredients then found = replace_ingredients(recipe.normal.ingredients, from, to) end
        if recipe.expensive and recipe.expensive.ingredients then found = replace_ingredients(recipe.expensive.ingredients, from, to) end
    end
end

function multiply_ingredients(root, factor)
	if not root then return nil end
	local ingredients = table.deepcopy(root)
	for _,i in pairs(ingredients) do
		if i.amount then i.amount = i.amount * factor
		else i[2] = i[2] * factor end
	end
	return ingredients
end

function multiply_results(results, factor)
	if not results then return nil end
	results = table.deepcopy(results)
	for _,r in pairs(results) do
		if r.amount then r.amount = r.amount * factor
		else r[2] = r[2] * factor end
	end
	return results
end

-- disable a recipe
function disable_recipe(recipe, state)
	if not data.raw.recipe[recipe] then return end
	if not state then state = false end
	if data.raw.recipe[recipe].normal then
		data.raw.recipe[recipe].normal.enabled = state
		data.raw.recipe[recipe].expensive.enabled = state
	else
		data.raw.recipe[recipe].enabled = state
	end
end

-- set single-result recipe count
function set_recipe_result_count(recipe, count)
   if not data.raw.recipe[recipe] then return end
   if data.raw.recipe[recipe].normal then
      data.raw.recipe[recipe].normal.result_count = count
      data.raw.recipe[recipe].expensive.result_count = count
   else
      data.raw.recipe[recipe].result_count = count
   end
end

-- disabled named recipes
function disable_recipes(recipelist)
    for _,recipe in pairs(recipelist) do
		disable_recipe(recipe)
    end
end

-- enable named recipes
function enable_recipes(recipelist)
    for _,recipe in pairs(recipelist) do
		disable_recipe(recipe, true)
    end
end

function hide_items(itemlist)
    for _,item in pairs(itemlist) do
		-- hide the item and recipe
		if data.raw.item[item] then data.raw.item[item].flags = {"hidden"}
		elseif data.raw.ammo[item] then data.raw.ammo[item].flags = {"hidden"}
		elseif data.raw["repair-tool"][item] then data.raw["repair-tool"][item].flags = {"hidden"}
		else error("Couldn't locate item "..item.." for hiding") end
		-- if this item places an entity, remove its next_upgrade
		local result = data.raw.item[item] and data.raw.item[item].place_result
		if result then
			for _,prototype in pairs(data.raw) do
				if prototype[result] and prototype[result].next_upgrade ~= nil then
					prototype[result].next_upgrade = nil
				end
			end
		end
		-- if there's a recipe with the same name, hide it tool
		if data.raw.recipe[item] then data.raw.recipe[item].hidden = true end
    end
end

-- replace a recipe's ingredients with a new set
function replace_recipe_ingredients(recipename, ingredients, result_count, category, ctime, result)
	if not result_count then result_count = 1 end
	local recipe = data.raw.recipe[recipename]
	if not recipe then return end
	local root = recipe.normal and {recipe.normal,recipe.expensive} or {recipe}
	for _,r in pairs(root) do
		r.results = nil
		r.ingredients = ingredients
		r.result = result or recipename
		r.result_count = result_count
		if ctime then r.energy_required = ctime
		elseif r.energy_required == nil then r.energy_required = DIR.default_crafting_time end
	end
	if category then recipe.category = category end
end

function get_crafting_ratio(ratio)
	if not ratio then return nil end
	return {from = ratio[1], to = ratio[2]}
end

function get_number_of_sources(component)
	return table_length(DIR.made_from[component])
end

function is_a_multi_recipe(recipename)
	return string.find(recipename,"-from-",1,true) and not string.find(recipename,"ingot-from-ingot",1,true)
end

function final_recipes_pass()
	for _,recipe in pairs(data.raw.recipe) do
		local for_fucks_sake = recipe.normal and {recipe.normal,recipe.expensive} or {recipe}
		for _,root in pairs(for_fucks_sake) do
			if root then
				if root.result and root.result_count and root.result_count > 1 then
					recipe.always_show_products = true
					recipe.show_amount_in_title = false
				end
			end
		end
	end
end

function ammo_pass()
	for _,ammo in pairs(data.raw.ammo) do
		local desc = {""}
		if ammo.reload_time then 
			table.insert(desc, {"item-description.reload-time", {"time-symbol-seconds-short",ammo.reload_time/60}})
		end
		if #desc > 1 then ammo.localised_description = desc end
	end
	for _,gun in pairs(data.raw.gun) do
		local desc = {""}
		if gun.attack_parameters then
			if gun.attack_parameters.ammo_category then 
				table.insert(desc, {"item-description.ammo-type", {"ammo-category-name."..gun.attack_parameters.ammo_category}})
			end
			if gun.attack_parameters.movement_slow_down_factor and gun.attack_parameters.movement_slow_down_factor > 0 then 
				if #desc > 1 then table.insert(desc, "\n") end
				table.insert(desc, {"item-description.movement-penalty", gun.attack_parameters.movement_slow_down_factor*100})
			end
		end
		if #desc > 1 then gun.localised_description = desc end
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------
