------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-items")
require("code.functions.functions-ammo")
require("code.functions.functions-recipes")
require("code.functions.functions-icons")
require("code.functions.functions-maths")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- GENERATED ITEMS

-- loop through material-component combinations, create items and recipes

local belt_variant_count = {}
for component,componentdata in pairs(DIR.components) do
	for _,material in pairs(componentdata.materials) do
		local materialdata = DIR.materials[material]
		if not materialdata then error("Component "..component.." specifies non-existent material "..material) end
		-- data used by both items and recipes
		local name = get_item_name(material, component)
		-- make the item
		local item = {}
		item.type = componentdata.type
		item.name = name
		item.stack_size = componentdata.stack_size or 100
		item.ir_component = true -- flag for detection later, discarded at end of data stage
		-- icon - does it have belt variants?
		local icon_name = name
		if not is_value_in_table(DIR.belt_variation_exclusions, name) and componentdata.belt_variations then
			if not belt_variant_count[component] then belt_variant_count[component] = 1
			elseif belt_variant_count[component] > 1 then icon_name = icon_name .. "-" .. belt_variant_count[component] end
			belt_variant_count[component] = belt_variant_count[component] + 1
			if belt_variant_count[component] > componentdata.belt_variations then belt_variant_count[component] = 1 end
		-- does it have predefined variant?
		elseif componentdata.variants and componentdata.variants[material] then
			icon_name = icon_name .. "-" .. componentdata.variants[material]
		end
		item.icon = get_item_icon(name, icon_name) 
		item.icon_size = DIR.icon_size
		-- crafting category - this is a recipe thing but needed for item subgroups as well
		-- does this component deviate from default crafting method?
		local item_category = (componentdata.crafting_exceptions and componentdata.crafting_exceptions[material]) or componentdata.category or "crafting"
		-- is it a tiered crafting category?
		if is_value_in_table(DIR.tiered_categories, item_category) then
			item_category = item_category .. "-" .. materialdata.tier
		end
		-- subgroup
		local item_subgroup = (componentdata.subgroup_exceptions and componentdata.subgroup_exceptions[material]) or componentdata.subgroup or component
		if componentdata.tabgroup ~= "deadlock-processing" and not is_it_player_craftable(item_category) then item_subgroup = "non-handcrafting" end
		item.subgroup = componentdata.item_subgroup or item_subgroup
		-- order
		item.order = string.format("aardvark-%s-%s", (componentdata.order_exceptions and componentdata.order_exceptions[material]) or componentdata.order, materialdata.order)
		-- fuels
		if DIR.component_fuel_values[name] then
			item.fuel_value = string.format("%gMJ", materialdata.base_fuel_value * DIR.component_fuel_values[name].multiplier)
			item.fuel_category = "chemical"
			if DIR.component_fuel_values[name].emissions_multiplier then item.fuel_emissions_multiplier = DIR.component_fuel_values[name].emissions_multiplier end
		end
		-- ammo
		if componentdata.type == "ammo" then
			if component == "magazine" then item.ammo_type = get_bullet_ammo_type(materialdata.base_damage, materialdata.hue, materialdata.saturation) 
			elseif component == "belt" then item.ammo_type = get_belt_ammo_type(materialdata.base_damage, materialdata.hue, materialdata.saturation) 
			elseif component == "cartridge" then item.ammo_type = get_cartridge_ammo_type(materialdata.base_damage, materialdata.pellets, materialdata.hue, materialdata.saturation)
			end
			item.magazine_size = DIR.ammo.size[component]
			item.reload_time = DIR.ammo.reload[component]
		end
		-- repair tools
		if componentdata.type == "repair-tool" then
			if not materialdata.repair_durability then error("Error: repair tool given material "..material.." which doesn't define durability") end
			item.durability = materialdata.repair_durability
			item.speed = materialdata.repair_speed or 2
		end
		-- fluids
		if componentdata.type == "fluid" then
			item.base_color = hsva2rgba(materialdata.hue,get_saturation(materialdata.hue,materialdata.saturation),1,1)
			item.flow_color = hsva2rgba(materialdata.hue,get_saturation(materialdata.hue,materialdata.saturation)*0.75,0.75,1)
			item.default_temperature = materialdata.default_temperature or 20
			item.max_temperature = materialdata.max_temperature or 100
			item.heat_capacity = "0.1KJ"
		end
		-- tiles
		if componentdata.tile_walking_speed and componentdata.tile_walking_speed[material] then
			item.place_as_tile = {
				condition = {"water-tile"},
				condition_size = (name == "stone-brick") and 1 or 2,
				result = (name == "stone-brick") and "stone-path" or name,
			}
		end
		-- "randomised" belt variations
		if not is_value_in_table(DIR.belt_variation_exclusions, name) and componentdata.belt_variations then
			if DIR.vanilla_icon_override[name] then
				item.pictures = data.raw.item[name].pictures
			else
				item.pictures = {}
				for i=1,componentdata.belt_variations do
					local variant_name = (i == 1) and item.name or item.name.."-"..i
					table.insert(item.pictures, {filename = get_icon_path(variant_name,DIR.icon_size), size = DIR.icon_size, scale = 0.25})
				end
			end
		end
		data:extend({item})
				
		-- make the recipe
		-- what is this component made from? is this specific material-component combo a special case?
		local made_from = (componentdata.made_from_exceptions and componentdata.made_from_exceptions[material]) or componentdata.made_from
		-- some components are never crafted i.e. ores
		if made_from then
			-- loop through all the source components for this item 
			for source,ratio in pairs(made_from) do
				if not ratio.amount or not ratio.result then error("Item "..name.." has badly specified source components") end
				-- crafting time - component speed, multiplied by material speed, optionally multiplied by number of outputs
				local base_speed = (componentdata.speed_exceptions and componentdata.speed_exceptions[material]) or componentdata.speed or 1.0
				local craft_time = base_speed * (materialdata.speed_multiplier or 1.0) * (componentdata.scale_speed and ratio.result or 1)
				-- order
				local order = string.format("aardvark-%s-%s-%s", (componentdata.order_exceptions and componentdata.order_exceptions[material]) or componentdata.order, materialdata.order, DIR.components[source].order)
				-- name of the recipe? if it has multiple sources, it gets a unique name, otherwise same name as result item
				local recipe_name = (table_length(made_from) == 1) and name or name.."-from-"..source
				-- if the source components exist, then make the recipe
				if is_value_in_table(DIR.components[source].materials, material) then 
					-- primary ingredients
					-- local recipe_category = (componentdata.crafting_exceptions and componentdata.crafting_exceptions[material]) or item_category
					local ingredients = {}
					if source ~= component then
						local source_name = get_item_name(material, source)
						if ratio.amount > 0 then
							if data.raw.fluid[source_name] then
								table.insert(ingredients, {type = "fluid", name = source_name, amount = ratio.amount})
								if item_category == "crafting" then item_category = "crafting-with-fluid" end
							else
								table.insert(ingredients, {type = "item", name = source_name, amount = ratio.amount})
							end
						end
					end
					-- secondary ingredients
					if componentdata.made_from_secondary and not (componentdata.made_from_exceptions and componentdata.made_from_exceptions[material]) then
						for secondary_comp, amount in pairs(componentdata.made_from_secondary) do
							if data.raw.fluid[secondary_comp] then
								table.insert(ingredients, {type = "fluid", name = secondary_comp, amount = amount, catalyst_amount = (secondary_comp == "water") and amount or nil})
								if item_category == "crafting" then item_category = "crafting-with-fluid" end
							else
								table.insert(ingredients, {type = "item", name = get_item_name(material, secondary_comp), amount = amount})
							end
						end
					end
					-- tertiary ingredients, per mat-comp combo
					if componentdata.made_from_tertiary and componentdata.made_from_tertiary[material] and not DIR.components[source].block_tertiary_ingredients then
						for tertiary_comp, amount in pairs(componentdata.made_from_tertiary[material]) do
							if data.raw.fluid[tertiary_comp] or string.find(tertiary_comp,"fluid") then
								table.insert(ingredients, {type = "fluid", name = tertiary_comp, amount = amount})
								if item_category == "crafting" then item_category = "crafting-with-fluid" end
							else
								table.insert(ingredients, {type = "item", name = tertiary_comp, amount = amount})
							end
						end
					end
					-- specific ingredients, per recipe
					if componentdata.made_from_specific and componentdata.made_from_specific[recipe_name] then
						for specific_comp, amount in pairs(componentdata.made_from_specific[recipe_name]) do
							if data.raw.fluid[specific_comp] or string.find(specific_comp,"fluid") then
								table.insert(ingredients, {type = "fluid", name = specific_comp, amount = amount})
								if item_category == "crafting" then item_category = "crafting-with-fluid" end
							else
								table.insert(ingredients, {type = "item", name = specific_comp, amount = amount})
							end
						end
					end
					-- tough materials
					local hardness = (materialdata.hardness_exceptions and materialdata.hardness_exceptions[component]) and materialdata.hardness_exceptions[component] or materialdata.hardness
					if hardness and hardness >= 5 and not componentdata.ignore_hardness and (item_category == "crafting" or item_category == "crafting-with-fluid") then
						item_category = (materialdata.tier < 5) and "advanced-crafting" or "endgame-crafting"
					end
					-- scrapping exclusions
					if componentdata.scrapping_exclusions and componentdata.scrapping_exclusions[material] then
						for exclusion_name,exclusion_amount in pairs(componentdata.scrapping_exclusions[material]) do
							for _,ingredient in pairs(ingredients) do
								if ingredient.name == exclusion_name then ingredient.scrapping_exclusion_amount = exclusion_amount end
							end
						end
					end
					-- emissions_multiplier
					local emissions = componentdata.source_emissions_multiplier and componentdata.source_emissions_multiplier[source] 
					-- Create the recipe
					local new_recipe = generate_recipe(recipe_name, name, icon_name, material, component, item_category, order, ratio.result, ingredients, craft_time, can_craft_at_start(material, component, source), icon_name, item_subgroup, emissions)
					-- Check to see if result needs probabilistic byproducts
					if componentdata.bonus_results and componentdata.bonus_results[material] then
						add_probability_to_recipe(new_recipe, item, component, componentdata.bonus_results[material], true, componentdata.tile_walking_speed and componentdata.tile_walking_speed[material])
						-- base layer icon per category
						if DIR.category_icons[item_category] then
							table.insert(new_recipe.icons, 1, {icon = get_icon_path(DIR.category_icons[item_category]), icon_size = DIR.icon_size})
						end
					end
					-- Components with multiple sources get distinguishing layered icons
					if (table_length(made_from) > 1) then
						if not materialdata.alloy or source ~= component then
							add_ingredient_icons_to_recipe(new_recipe)
						else
							convert_recipe_icon_to_icons(new_recipe)
							add_icons_to_recipe(new_recipe,{{icon = get_icon_path(material.."-alloying"), icon_size = DIR.icon_size}}, -1)
						end
					end
					-- Add to productivity list
					if componentdata.productivity then table.insert(DIR.productivity_limitation,recipe_name) end
					-- duplication e.g. advanced washing
					if componentdata.duplication then
						recipe_name = componentdata.duplication.prefix .. new_recipe.name
						local new_ingredients = table.deepcopy(ingredients)
						-- replace specific ingredients in a proportion
						if componentdata.duplication.replace_ingredients then
							for old,new in pairs(componentdata.duplication.replace_ingredients) do
								local multiplier = componentdata.duplication.replace_multiplier[old] or 1
								for _,ingredient in pairs(new_ingredients) do
									if ingredient.name == old then
										if multiplier == 1 then ingredient.name = new
										else
											table.insert(new_ingredients, {name = new, type = ingredient.type, amount = ingredient.amount * multiplier})
											ingredient.amount = ingredient.amount * (1-multiplier)
										end
									end
								end
							end
						end
						local duplicate = generate_recipe(recipe_name, name, icon_name, material, component, componentdata.duplication.prefix..item_category, "zz-"..order, ratio.result, new_ingredients, craft_time * componentdata.duplication.time_multiplier, can_craft_at_start(material, component, source), icon_name, item_subgroup, emissions)
						if componentdata.bonus_results and componentdata.bonus_results[material] then
							add_probability_to_recipe(duplicate, item, component, componentdata.bonus_results[material], true, componentdata.tile_walking_speed and componentdata.tile_walking_speed[material], componentdata.duplication.result_probability, componentdata.duplication.byproduct_probability_multiplier)
							if DIR.category_icons[componentdata.duplication.prefix..item_category] then
								table.insert(duplicate.icons, 1, {icon = get_icon_path(DIR.category_icons[componentdata.duplication.prefix..item_category]), icon_size = DIR.icon_size})
							end
						end
						if componentdata.productivity then table.insert(DIR.productivity_limitation,recipe_name) end
					end
				end
			end
		end
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------
