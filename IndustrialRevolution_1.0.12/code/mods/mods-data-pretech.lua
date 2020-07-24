------------------------------------------------------------------------------------------------------------------------------------------------------

-- PRE-TECH MODS
-- mods which need support before tree build runs and before techs are wiped, e.g. setting new ingredients

------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-recipes")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- DSB/DCM

if mods["deadlock-beltboxes-loaders"] then 
	if settings.startup["deadlock-enable-loaders"].value then
		replace_recipe_ingredients("transport-belt-loader", {{"copper-chassis-small", 1}, {"tin-gear-wheel",2}, {"transport-belt", 1}, {"tin-rod",4}})
		replace_recipe_ingredients("fast-transport-belt-loader", {{"iron-chassis-small", 1}, {"iron-gear-wheel",2}, {"fast-transport-belt", 1}, {"iron-stick",4}})
		replace_recipe_ingredients("express-transport-belt-loader", {{"steel-chassis-small", 1}, {"steel-gear-wheel",2}, {"express-transport-belt", 1}, {"steel-rod",4}})
		data.raw.recipe["transport-belt-loader"].order = "za"
		data.raw.recipe["fast-transport-belt-loader"].order = "zb"
		data.raw.recipe["express-transport-belt-loader"].order = "zc"
	end
	if settings.startup["deadlock-enable-beltboxes"].value then
		replace_recipe_ingredients("transport-belt-beltbox", {{"iron-chassis-small", 1}, {"transport-belt", 1}, {"iron-gear-wheel",4}, {"electronic-circuit",1}})
		replace_recipe_ingredients("fast-transport-belt-beltbox", {{"steel-chassis-small", 1}, {"fast-transport-belt", 1}, {"steel-gear-wheel",4}, {"advanced-circuit",1}})
		replace_recipe_ingredients("express-transport-belt-beltbox", {{"titanium-chassis-small", 1}, {"express-transport-belt", 1}, {"titanium-gear-wheel",4}, {"processing-unit",1}}, 1, "crafting", 1)
		data.raw.recipe["transport-belt-beltbox"].subgroup = "loaders"
		data.raw.recipe["fast-transport-belt-beltbox"].subgroup = "loaders"
		data.raw.recipe["express-transport-belt-beltbox"].subgroup = "loaders"
		data.raw.recipe["transport-belt-beltbox"].order = "zd"
		data.raw.recipe["fast-transport-belt-beltbox"].order = "ze"
		data.raw.recipe["express-transport-belt-beltbox"].order = "zf"
	end
end

if mods["DeadlockCrating"] then
	replace_recipe_ingredients("deadlock-crating-machine-1", {{"steel-chassis-large", 1}, {"stack-inserter", 2}, {"transport-belt",8}, {"controller-mk1",1}})
	replace_recipe_ingredients("deadlock-crating-machine-2", {{"steel-chassis-large", 1}, {"stack-inserter", 2}, {"fast-transport-belt",8}, {"controller-mk2",1}})
	replace_recipe_ingredients("deadlock-crating-machine-3", {{"steel-chassis-large", 1}, {"stack-inserter", 2}, {"express-transport-belt",8}, {"controller-mk3",1}})
	if mods["deadlock-beltboxes-loaders"] then
		data.raw.recipe["deadlock-crating-machine-1"].subgroup = "loaders"
		data.raw.recipe["deadlock-crating-machine-2"].subgroup = "loaders"
		data.raw.recipe["deadlock-crating-machine-3"].subgroup = "loaders"
		data.raw.recipe["deadlock-crating-machine-1"].order = "zg"
		data.raw.recipe["deadlock-crating-machine-2"].order = "zh"
		data.raw.recipe["deadlock-crating-machine-3"].order = "zi"
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- Loader Redux

if mods["LoaderRedux"] then 
	replace_recipe_ingredients("loader", {{"copper-chassis-small", 1}, {"tin-gear-wheel",4}, {"transport-belt", 2}, {"tin-plate",4}})
	replace_recipe_ingredients("fast-loader", {{"iron-chassis-small", 1}, {"iron-gear-wheel",4}, {"fast-transport-belt", 2}, {"iron-plate",4}})
	replace_recipe_ingredients("express-loader", {{"steel-chassis-small", 1}, {"steel-gear-wheel",4}, {"express-transport-belt", 2}, {"steel-plate",4}})
end

------------------------------------------------------------------------------------------------------------------------------------------------------