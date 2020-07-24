------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-entities")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- return a formatted icon path

function get_icon_path(name, icon_size)
	if not icon_size then icon_size = DIR.icon_size end
	return string.format("%s/%d/%s.png", DIR.icon_path, icon_size, name)
end

-- replace an item's icon

function replace_item_icon(item_name, icon)
	if not data.raw.item[item_name] and not data.raw.fluid[item_name] and not data.raw.tool[item_name] and not data.raw.module[item_name] then
		error("Asked to update icon for non-existent item/fluid/tool/module "..item_name)
		return
	end
	if not icon then icon = item_name end
	local item = data.raw.item[item_name] or data.raw.fluid[item_name] or data.raw.tool[item_name] or data.raw.module[item_name]
	item.icon = get_icon_path(icon, DIR.icon_size)
	item.icons = nil
	item.icon_size = DIR.icon_size
	if item.place_result then
		local entity = wild_stab_at_entity_by_name(item.place_result)
		if entity then
			entity.icon = item.icon
			entity.icon_size = item.icon_size
		end
	end
end

function replace_recipe_icon(recipe_name, icon)
	if not data.raw.recipe[recipe_name] then
		error("Asked to update icon for non-existent recipe "..recipe_name)
		return
	end
	if not icon then icon = recipe_name end
	local recipe = data.raw.recipe[recipe_name]
	recipe.icon = get_icon_path(icon, DIR.icon_size)
	recipe.icons = nil
	recipe.icon_size = DIR.icon_size
end

-- replace a tech's icon

function replace_tech_icon(tech_name, icon)
	if not data.raw.technology[tech_name] then
		error("Asked to update icon for non-existent item "..tech_name)
		return
	end
	if not icon then icon = tech_name end
	data.raw.technology[tech_name].icon = get_icon_path(icon, 128)
	data.raw.technology[tech_name].icons = nil
	data.raw.technology[tech_name].icon_size = 128
end

-- get variants of an item's icon

function get_item_icon(name, icon)
	if DIR.vanilla_icon_override[name] then return string.format("__base__/graphics/icons/mip/%s.png", name) end
	return get_icon_path(icon, DIR.icon_size)
end

function convert_recipe_icon_to_icons(recipe)
	if recipe.icons then return end
	if not recipe.icon then 
		recipe.icons = {}
		return
	end
	recipe.icons = {
		{
			icon = recipe.icon,
			icon_size = recipe.icon_size,
		},
	}
	recipe.icon = nil
	recipe.icon_size = nil
end

function is_icon_in_icons(icon,icons)
	for _,this_icon in pairs(icons) do
		if this_icon.icon == icon then return true end
	end
	return false
end

function add_icons_to_recipe(recipe, newicons, dy, x)
	if x == nil then x = -8 end
	local number = math.max(2,#newicons)
	local scale = 0.5
	local count = 1
	for _,this_icon in pairs(newicons) do
		table.insert(recipe.icons, {
			icon = this_icon.icon,
			icon_size = this_icon.icon_size,
			scale = scale * (32/this_icon.icon_size),
			shift = { linear_transform(count,1,number,x,8), dy * 8 },
		})
		count = count + 1
	end
end

function add_ingredient_icons_to_recipe(recipe)
	convert_recipe_icon_to_icons(recipe)
	local newicons = {}
	for _,ingredient in pairs(recipe.ingredients) do
		local name = ingredient.name or ingredient[1]
		if ingredient.type == "fluid" then table.insert(newicons, { icon = data.raw.fluid[name].icon, icon_size = data.raw.fluid[name].icon_size })
		else table.insert(newicons, { icon = data.raw.item[name].icon, icon_size = data.raw.item[name].icon_size })
		end
	end
	add_icons_to_recipe(recipe,newicons,-1)
end

function add_result_icons_to_recipe(recipe,ignore)
	convert_recipe_icon_to_icons(recipe)
	local newicons = {}
	if recipe.results then
		for _,result in pairs(recipe.results) do
			local name = result.name or result[1]
			if name ~= recipe.main_product and (not ignore or name ~= ignore) then
				table.insert(newicons, { icon = get_icon_path(name,DIR.icon_size), icon_size = DIR.icon_size })
			end
		end
	end
	add_icons_to_recipe(recipe,newicons,1)
end

------------------------------------------------------------------------------------------------------------------------------------------------------
