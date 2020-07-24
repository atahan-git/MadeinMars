------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-log")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- TECHNOLOGY WORK-OVER PART 2

-- AUTOMATED RECURSIVE PREREQUISITE ADJUSTMENT
-- In the Revolution, if a technology unlocks a recipe, then all the *ingredients* for all of those recipes *must* already have been unlocked
-- so either a recipe is enabled at game start, or it is chained from a previous research
-- method: scan every technology and create a list of all non-enabled ingredients used by its unlocks
-- then for every ingredient, find the tech that unlocks it, and add it to the prereqs
-- finally, remove all duplications and redundancies

------------------------------------------------------------------------------------------------------------------------------------------------------

-- populate look ups, recipes <> technologies

local tech_requires_items = {}
local tech_provides_items = {}
local item_provided_by_techs = {}

for _,tech in pairs(data.raw.technology) do
	if tech.enabled ~= false and tech.effects and not is_an_ignored_technology(tech.name) then
		tech_requires_items[tech.name] = {} 
		for _,effect in pairs(tech.effects) do
			if effect.type == "unlock-recipe" and not is_an_ignored_recipe(effect.recipe) then
				local recipe = data.raw.recipe[effect.recipe]
				if not recipe then error("Tech contains non-existent recipe unlock: "..tech.name..", "..effect.recipe) end
				local ingredients = nil
				if recipe.normal then ingredients = recipe.normal.ingredients else ingredients = recipe.ingredients end
				if ingredients then
					for _,ingredient in pairs(ingredients) do
						local name = ingredient.name
						if not name then name = ingredient[1] end
						if not is_value_in_table(DIR.ancestor_ingredient_ignore_list, name) and not is_value_in_table(tech_requires_items[tech.name], name) then table.insert(tech_requires_items[tech.name], name) end
					end
				end
				if not is_a_multi_recipe(effect.recipe) then
					local recipe_root = recipe.normal or recipe
					if recipe_root.result then 
						if not tech_provides_items[tech.name] then tech_provides_items[tech.name] = {} end 
						table.insert(tech_provides_items[tech.name], recipe_root.result) 
						if not item_provided_by_techs[recipe_root.result] then item_provided_by_techs[recipe_root.result] = {} end 
						table.insert(item_provided_by_techs[recipe_root.result], tech.name) 
					end
					if recipe_root.results then 
						for _,result in pairs(recipe_root.results) do
							local name = result.name
							if (not result.probability or DIR.tech_search_allowed_probability_components[name]) then
								if not name then name = result[1] end
								if not tech_provides_items[tech.name] then tech_provides_items[tech.name] = {} end 
								table.insert(tech_provides_items[tech.name], name) 
								if not item_provided_by_techs[name] then item_provided_by_techs[name] = {} end 
								table.insert(item_provided_by_techs[name], tech.name) 
							end
						end
					end
				end
			end
		end
		-- clean up any duplicates
		local temp = {}
		for tech,items in pairs(tech_requires_items) do
			for _,item in pairs(items) do
				if not list_contains(tech_provides_items[tech],item) then 
					if not temp[tech] then temp[tech] = {} end
					table.insert(temp[tech], item)
				end
			end
		end
		tech_requires_items = temp
		--if tech_requires_items[tech.name] then spam_log(string.format("Technology %s requires: %s", tech.name, serialise(tech_requires_items[tech.name]))) end
		--if tech_provides_items[tech.name] then spam_log(string.format("Technology %s provides: %s\n", tech.name, serialise(tech_provides_items[tech.name]))) end
	end
end

-- for every ingredient needed, get the tech that provides it and add it as a pre-req
-- if multiple techs provide it, add them all - we already have to remove redundancies later anyway

for tech,ingredients in pairs(tech_requires_items) do
	local tech_prototype = data.raw.technology[tech]
	local added_providers = {}
	for _,ingredient in pairs(ingredients) do
		local providers = item_provided_by_techs[ingredient]
		if providers then 
			for _,provider in pairs(providers) do
				if provider ~= tech and not list_contains(tech_prototype.prerequisites, provider) then 
					if not tech_prototype.prerequisites then tech_prototype.prerequisites = {} end
					table.insert(tech_prototype.prerequisites, provider)
					table.insert(added_providers, provider)
				end
			end	
		end
	end
	--if table_length(added_providers) > 0 then spam_log(string.format("Added pre-requisites to %s: %s", tech, serialise(added_providers))) end
end

-- then rescan the prereqs and remove redundancies

for _,tech in pairs(data.raw.technology) do
	if tech.enabled ~= false and tech.prerequisites then
		local ancestors = {}
		for _,prerequisite in pairs(tech.prerequisites) do
			if not data.raw.technology[prerequisite] then error("Error: tech "..tech.name.." has non-existent pre-requisite "..prerequisite) end
			if data.raw.technology[prerequisite].enabled ~= false then 
				ancestors = merge_keys(ancestors, get_ancestors_of_tech(prerequisite))
			end
		end
		local parents = values_to_keys(tech.prerequisites)
		local removed_providers = {}
		for parent,_ in pairs(parents) do
			if ancestors[parent] then
				parents[parent] = nil
				table.insert(removed_providers, parent)
			end			
		end
		tech.prerequisites = keys_to_values(parents)
		--if table_length(removed_providers) > 0 then spam_log(string.format("Removed pre-requisites from %s: %s", tech.name, serialise(removed_providers))) end
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------
