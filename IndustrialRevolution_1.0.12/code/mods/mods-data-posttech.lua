------------------------------------------------------------------------------------------------------------------------------------------------------

-- POST-TECH MODS
-- mods which need support after tree build runs e.g. setting new ingredients

------------------------------------------------------------------------------------------------------------------------------------------------------

-- Deadlock's Larger Lamp

if mods["DeadlockLargerLamp"] then
	replace_recipe_ingredients("deadlock-large-lamp", {{"electronic-circuit", 1}, {"iron-chassis-small", 1}, {"glass-plate", 8}, {"copper-cable", 8}})
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- Text Plates

if mods["textplates"] then
	data.raw.recipe["textplate-large-iron"].enabled = false
	data.raw.recipe["textplate-small-iron"].enabled = false
	table.insert(data.raw.technology["deadlock-iron-age"].effects, { type = "unlock-recipe", recipe = "textplate-large-iron" })
	table.insert(data.raw.technology["deadlock-iron-age"].effects, { type = "unlock-recipe", recipe = "textplate-small-iron" })
	data.raw.recipe["textplate-large-gold"].enabled = false
	data.raw.recipe["textplate-small-gold"].enabled = false
	table.insert(data.raw.technology["deadlock-iron-age"].effects, { type = "unlock-recipe", recipe = "textplate-large-gold" })
	table.insert(data.raw.technology["deadlock-iron-age"].effects, { type = "unlock-recipe", recipe = "textplate-small-gold" })
	data.raw.recipe["textplate-large-steel"].enabled = false
	data.raw.recipe["textplate-small-steel"].enabled = false
	table.insert(data.raw.technology["deadlock-steel-age"].effects, { type = "unlock-recipe", recipe = "textplate-large-steel" })
	table.insert(data.raw.technology["deadlock-steel-age"].effects, { type = "unlock-recipe", recipe = "textplate-small-steel" })
	data.raw.recipe["textplate-large-uranium"].enabled = false
	data.raw.recipe["textplate-small-uranium"].enabled = false
	table.insert(data.raw.technology["uranium-processing"].effects, { type = "unlock-recipe", recipe = "textplate-large-uranium" })
	table.insert(data.raw.technology["uranium-processing"].effects, { type = "unlock-recipe", recipe = "textplate-small-uranium" })
	data.raw.recipe["textplate-large-concrete"].enabled = false
	data.raw.recipe["textplate-small-concrete"].enabled = false
	table.insert(data.raw.technology["deadlock-concrete-1"].effects, { type = "unlock-recipe", recipe = "textplate-large-concrete" })
	table.insert(data.raw.technology["deadlock-concrete-1"].effects, { type = "unlock-recipe", recipe = "textplate-small-concrete" })
	data.raw.recipe["textplate-large-glass"].enabled = false
	data.raw.recipe["textplate-small-glass"].enabled = false
	data.raw.recipe["textplate-large-glass"].ingredients = {{type = "item", name = "glass-plate", amount = 4}}
	data.raw.recipe["textplate-small-glass"].ingredients = {{type = "item", name = "glass-plate", amount = 1}}
	table.insert(data.raw.technology["deadlock-grinding-1"].effects, { type = "unlock-recipe", recipe = "textplate-large-glass" })
	table.insert(data.raw.technology["deadlock-grinding-1"].effects, { type = "unlock-recipe", recipe = "textplate-small-glass" })
	data.raw["item-subgroup"]["textplates"].group = mods["Dectorio"] and "dectorio" or "logistics"
	data.raw["item-subgroup"]["textplates"].order = "zzz"
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- More Text Plates

if mods["DeadlockMoreTextPlates"] then
	data.raw.recipe["textplate-large-plastic"].enabled = false
	data.raw.recipe["textplate-small-plastic"].enabled = false
	data.raw.recipe["textplate-large-plastic"].ingredients = {{type = "item", name = "plastic-bar", amount = 4}}
	data.raw.recipe["textplate-small-plastic"].ingredients = {{type = "item", name = "plastic-bar", amount = 1}}
	table.insert(data.raw.technology["plastics"].effects, { type = "unlock-recipe", recipe = "textplate-large-plastic" })
	table.insert(data.raw.technology["plastics"].effects, { type = "unlock-recipe", recipe = "textplate-small-plastic" })
	data.raw.recipe["textplate-large-plasticcoloured"].enabled = false
	data.raw.recipe["textplate-small-plasticcoloured"].enabled = false
	data.raw.recipe["textplate-large-plasticcoloured"].ingredients = {{type = "item", name = "plastic-bar", amount = 4}}
	data.raw.recipe["textplate-small-plasticcoloured"].ingredients = {{type = "item", name = "plastic-bar", amount = 1}}
	table.insert(data.raw.technology["plastics"].effects, { type = "unlock-recipe", recipe = "textplate-large-plasticcoloured" })
	table.insert(data.raw.technology["plastics"].effects, { type = "unlock-recipe", recipe = "textplate-small-plasticcoloured" })
	data.raw["item-subgroup"]["deadlock-moretextplates"].group = mods["Dectorio"] and "dectorio" or "logistics"
end

------------------------------------------------------------------------------------------------------------------------------------------------------


