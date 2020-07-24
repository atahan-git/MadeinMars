------------------------------------------------------------------------------------------------------------------------------------------------------

-- generate name from mat-comp combo - check overrides

function get_item_name(material, component)
	local name = string.format("%s-%s", material, component)
	return DIR.name_overrides[name] or name
end

-- can the player craft this category

function is_it_player_craftable(category)
	if category == "electroplating" then return true end
	for _,v in pairs(data.raw.character.character.crafting_categories) do
		if v == category then return true end
	end
	return false
end

-- can I craft it at game start?

function can_craft_at_start(material, component, source)
	if get_item_name(material,component) == "stone-brick" then return true end -- ugly but name kludge to fit with vanilla
	if source and not DIR.components[source].start then return false end
	if DIR.materials[material].start and ((DIR.components[component].start_exceptions and DIR.components[component].start_exceptions[material]) or DIR.components[component].start) then return true end
	return false
end

-- move an item to a different subgroup

function change_item_subgroup(name, subgroup)
	if not data.raw.item[name] then
		error("Asked to change subgroup for non-existent item "..name)
		return
	end
	data.raw.item[name].subgroup = subgroup
end

-- change item crafting tab order

function change_item_order(name, order)
	if not data.raw.item[name] then
		error("Asked to change order for non-existent item "..name)
		return
	end
	data.raw.item[name].order = order
end

function change_recipe_order(name, order)
	if not data.raw.recipe[name] then
		error("Asked to change order for non-existent recipe "..name)
		return
	end
	data.raw.recipe[name].order = order
end

function change_item_and_recipe_order(name, order)
	change_item_order(name, order)
	change_recipe_order(name, order)
end

-- move a recipe to a different subgroup

function change_recipe_subgroup(name, subgroup)
	if not data.raw.recipe[name] then
		error("Asked to change subgroup for non-existent item "..name)
		return
	end
	data.raw.recipe[name].subgroup = subgroup
end

-- change a recipe's crafting category

function change_recipe_category(name, category)
	if not data.raw.recipe[name] then
		error("Asked to change category for non-existent recipe "..name)
		return
	end
	data.raw.recipe[name].category = category
end

function get_item_of_any_type(name)
	for _,type in pairs({"item","module","tool","repair-tool","ammo","capsule","gun","rail-planner","armor","item-with-entity-data","blueprint","blueprint-book","upgrade-item","deconstruction-item"}) do
		if data.raw[type][name] then return data.raw[type][name] end
	end
	return nil
end

------------------------------------------------------------------------------------------------------------------------------------------------------

