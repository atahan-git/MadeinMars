------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-log")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- does a tech unlock any recipes?

function does_tech_unlock_recipes(tech)
	if not tech.effects then return false end
	for _,effect in pairs(tech.effects) do
		if effect.type == "unlock-recipe" then return true end
	end
	return false
end

-- create a bare-bones new tech

function add_new_minimal_tech(tech_name, icon, vanilla_icon, unlocks, prerequisites, bonus_effects)
    if vanilla_icon then icon = string.format("__base__/graphics/technology/%s.png",icon) else icon = get_icon_path(icon, 128) end
    local effects = {}
    if unlocks then
        for _,unlock in pairs(unlocks) do
            table.insert(effects, {
                type = "unlock-recipe",
                recipe = unlock
            })
        end
    end
    if bonus_effects then
        for _,effect in pairs(bonus_effects) do
            table.insert(effects, effect)
        end
    end
    -- wipe the data first in case it exists in vanilla
    data.raw.technology[tech_name] = nil
    data:extend {{
        type = "technology",
        name = tech_name,
        enabled = true,
        icon = icon,
        icon_size = 128,
        unit = {
            count = 999,
            time = 60,
            ingredients = {{"automation-science-pack",1}},
        },
        prerequisites = prerequisites,
        effects = effects,
        upgrade = false,
		ir_native = true, -- so we know that this is native
    }}
    return data.raw.technology[tech_name]
end

-- add a machine masked icon to a tech

function add_machine_mask_to_tech(tech, machine)
    tech.icon = nil
    tech.icon_size = nil
    tech.icons = {
        { icon = get_icon_path(DIR.icon_masks[machine].base,128), icon_size = 128 },
        { icon = get_icon_path(DIR.icon_masks[machine].mask,128), icon_size = 128, tint = DIR.icon_masks[machine].tint },
    }
end

-- return the first tech you find that unlocks a recipe

function find_tech_that_unlocks(recipe)
    for _,tech in pairs(data.raw.technology) do
        if tech.enabled ~= false and tech.effects then
            for _,effect in pairs(tech.effects) do
                if effect.recipe == recipe then return tech end
            end
        end
    end
    return nil
end

-- return a list of tech names that have this tech as prereq

function find_techs_with_prereq(prereq)
    local techs = {}
    for _,tech in pairs(data.raw.technology) do
        if tech.enabled ~= false and tech.prerequisites and is_value_in_table(tech.prerequisites, prereq) then table.insert(techs, tech.name) end
    end
    return techs
end

-- add recipe unlock to tech

function add_unlock_to_tech(tech,recipe,index)
    local t = data.raw.technology[tech]
    if not t then error("Asked to add unlock to non-existent tech "..tech) end
    if not t.effects then t.effects = {} end
    if not index then table.insert(t.effects, {type = "unlock-recipe", recipe = recipe})
    else table.insert(t.effects, index, {type = "unlock-recipe", recipe = recipe}) end
end

-- strip recipe unlock from tech

function remove_unlock_from_tech(tech, recipe)
    local t = data.raw.technology[tech]
    if not t then error("Asked to remove unlock from non-existent recipe "..tech) end
    if not t.effects then return end
    for k,effect in pairs(t.effects) do
        if effect.recipe == recipe then
            t.effects[k] = nil
            return
        end
    end
end

-- remove all effects in a tech

function remove_all_effects_from_tech(tech)
    local t = data.raw.technology[tech]
    if not t or not t.effects then return end
    t.effects = {}
end

-- remove all barrel recipes from a tech

function remove_all_barrels_from_tech(tech)
    local t = data.raw.technology[tech]
    if not t or not t.effects then return end
    local copy = {}
    for _,effect in pairs(t.effects) do
        if effect.recipe and not string.find(effect.recipe,"%-barrel$") then table.insert(copy,effect) end
    end
    t.effects = copy
end

-- does tech have a specific prereq

function get_tech_has_prereq(tech_name, prereq)
	if data.raw.technology[tech_name] or not data.raw.technology[tech_name].prerequisites then return false end
	for _,p in pairs(data.raw.technology[tech_name].prerequisites) do
		if p == prereq then return true end
	end
	return false
end

-- add a new prereq to a tech

function add_prereq_to_tech(tech_name, prereq)
    if not data.raw.technology[tech_name] then error("Asked to add prereqs to non-existent tech: "..tech_name) end
    if not data.raw.technology[tech_name].prerequisites then data.raw.technology[tech_name].prerequisites = {} end
	if not get_tech_has_prereq(tech_name, prereq) then table.insert(data.raw.technology[tech_name].prerequisites, prereq) end
end

-- override all prereqs in a tech

function replace_all_prereqs(tech, prereq)
    data.raw.technology[tech].prerequisites = prereq
end

-- remove a specific prereq from a tech

function remove_prereq_from_tech(tech, prereq)
    if not data.raw.technology[tech] or not data.raw.technology[tech].prerequisites then return end
    local t = {}
    for _,v in pairs(data.raw.technology[tech].prerequisites) do
        if v ~= prereq then table.insert(t,v) end
    end
    data.raw.technology[tech].prerequisites = t
end

-- disable a tech - also remove it as a prereq from all other techs

function disable_technology(tech)
    tech = data.raw.technology[tech]
    tech.enabled = false
    tech.hidden = true
    tech.prerequisites = nil
    for _,rawtech in pairs(data.raw.technology) do
        remove_prereq_from_tech(rawtech.name, tech)
    end
end

function enable_technology(tech)
    tech = data.raw.technology[tech]
    tech.enabled = true
    tech.hidden = false
end

-- add a science pack ingredient to a tech

function add_pack_to_tech(pack, tech_name)
    local tech = data.raw.technology[tech_name]
    if not tech then error("Asked to add science pack to non-existent tech: "..tech_name) end
    if not tech.unit.ingredients then tech.unit.ingredients = {} end
    -- make sure it's not already there
    for _,ingredient in pairs(tech.unit.ingredients) do
        if ingredient[1] == pack then return end
    end
    table.insert(tech.unit.ingredients, { pack, 1 } )
end

-- add a science pack ingredient to a tech and all its "children"
-- assumes that cycles have already been identified and aborted from

function recursive_add_pack_to_tech(pack, tech_name,count)
    if not count then count = 1 end
    add_pack_to_tech(pack, tech_name)
    local children = find_techs_with_prereq(tech_name)
    for _,child in pairs(children) do
        recursive_add_pack_to_tech(pack,child)
    end
end

-- check if a tech uses a pack

function get_tech_has_pack(tech,pack)
	if tech.unit and tech.unit.ingredients then
		for _,ingredient in pairs(tech.unit.ingredients) do
			if ingredient[1] == pack then return true end
		end
	end
	return false
end

-- remove a pack from a tech

function remove_pack_from_tech(tech,pack)
	if tech.unit and tech.unit.ingredients then
		for k,ingredient in pairs(tech.unit.ingredients) do
			if ingredient[1] == pack then
				table.remove(tech.unit.ingredients,k)
				return
			end
		end
	end
end

-- recursively search techs for ancestors and populate the cache

function get_ancestors_of_tech(tech_name, chain)
    local proto = data.raw.technology[tech_name]
    if not proto or not proto.prerequisites or proto.enabled == false then return nil end

	-- we may have trodden this road before
    if DIR.ancestors[tech_name] then return DIR.ancestors[tech_name] end

	-- build up list of names
	if not chain then chain = {tech_name}
	elseif is_value_in_table(chain,tech_name) then
		-- we're in a cycle. abort, abort
		table.insert(chain,tech_name)
		local message = "\n\n[color=red][font=default-bold]Guru meditation error: Industrial Revolution has detected a cycle in the technology tree.[/font][/color]\nProbable cause: another mod has added an incompatible recipe unlock or technology prerequisite to one of the following:\n\n"
		local found = false
		for _,name in pairs(chain) do
			if name == tech_name then found = true end
			if found then
				message = message..name.."\n"
			end
		end
		error(message,0)
	else table.insert(chain,tech_name) end

	-- flip prereqs list into keys
    local ancestors = values_to_keys(proto.prerequisites)
	
	-- for each prereq, merge in their ancestors' prereqs, recursively
    for prerequisite,_ in pairs(values_to_keys(proto.prerequisites)) do
        ancestors = merge_keys(ancestors, get_ancestors_of_tech(prerequisite, chain))
    end
	
	-- cache this result so we don't have to do the same thing over and over
    DIR.ancestors[tech_name] = ancestors
    return ancestors
end

------------------------------------------------------------------------------------------------------------------------------------------------------
