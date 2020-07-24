------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-items")
require("code.functions.functions-recipes")
require("code.functions.functions-icons")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- update vanilla recipes with DIR scheme

------------------------------------------------------------------------------------------------------------------------------------------------------

-- disable/enable some of the default recipes

disable_recipes({
	"stone-furnace",
    "iron-chest",
    "inserter",
    "burner-mining-drill",
    "small-electric-pole",
    "pipe",
    "pipe-to-ground",
    "offshore-pump",
    "repair-pack",
    "boiler",
    "steam-engine",
    "electric-mining-drill",
    "lab",
    "electronic-circuit",
    "radar",
    "firearm-magazine",
	"advanced-oil-processing",
})

enable_recipes({
    "shotgun",
})

hide_items({
	"repair-pack",
	"firearm-magazine",
	"piercing-rounds-magazine",
	"uranium-rounds-magazine",
	"shotgun-shell",
	"piercing-shotgun-shell",
	"stone-furnace",
	"steel-furnace",
	"electric-furnace",
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- science packs 
replace_recipe_ingredients("automation-science-pack", {{"copper-plate", 1}, {"tin-gear-wheel", 2}}, 1, "crafting", 4)
replace_recipe_ingredients("logistic-science-pack", {{"bronze-tube", 2}, {"glass-ingot", 2}}, 1, "crafting", 4)
replace_recipe_ingredients("chemical-science-pack", {{"plastic-bar", 2}, {"solid-fuel", 1}, {"iron-gravel", 5}, {"copper-gravel",5}}, 1, "crafting", 8)
replace_recipe_ingredients("military-science-pack", {{"iron-magazine", 1}, {"copper-cartridge",1}, {"carbon-powder", 3}, {"sand", 6}}, 1, "crafting", 6)
replace_recipe_ingredients("production-science-pack", {{"advanced-circuit", 2}, {"concrete", 20}, {"steel-chassis-large",1}, {"titanium-plate-heavy",2}}, 1, "crafting", 8)
replace_recipe_ingredients("utility-science-pack", {{"processing-unit", 1}, {"low-density-structure", 1}, {"battery", 2}, {"steel-ring",1}}, 1, "crafting", 8)

-- belts
replace_recipe_ingredients("transport-belt", {{"tin-plate", 1}, {"copper-rod", 1}, {"copper-gear-wheel", 2}}, 2)
replace_recipe_ingredients("underground-belt", {{"tin-plate", 4}, {"transport-belt", 5}}, 2)
replace_recipe_ingredients("splitter", {{"tin-plate", 2}, {"copper-gear-wheel", 4}, {"transport-belt", 2}})
replace_recipe_ingredients("fast-transport-belt", {{"rubber-natural",1}, {"iron-plate", 1}, {"iron-stick", 1}, {"iron-gear-wheel", 2}})
replace_recipe_ingredients("fast-underground-belt", {{"iron-plate", 4}, {"fast-transport-belt", 7}}, 2)
replace_recipe_ingredients("fast-splitter", {{"iron-plate", 2}, {"iron-gear-wheel", 4}, {"fast-transport-belt", 2}, {"electronic-circuit", 1}})
replace_recipe_ingredients("express-transport-belt", {{"rubber-vulcanised",1}, {type = "item", name = "steel-plate", amount = 1}, {type = "item", name = "steel-rod", amount = 1}, {type = "item", name = "steel-gear-wheel", amount = 2}, {type = "fluid", name = "lubricant", amount = 20}}, 1, "crafting-with-fluid")
replace_recipe_ingredients("express-underground-belt", {{"steel-plate", 4}, {"express-transport-belt", 9}}, 2, "crafting")
replace_recipe_ingredients("express-splitter", {{"steel-plate", 2}, {"steel-gear-wheel", 4}, {"express-transport-belt", 2}, {"controller-mk2", 1}}, 1, "crafting")

-- assemblers
replace_recipe_ingredients("assembling-machine-1", {{"copper-chassis-large", 1}, {"burner-inserter", 2}, {"tin-gear-wheel", 8}}, 1, "crafting", 3)
replace_recipe_ingredients("assembling-machine-2", {{"iron-chassis-large", 1}, {"inserter", 2}, {"iron-gear-wheel", 8}, {"controller-mk1", 1}}, 1, "crafting", 5)
replace_recipe_ingredients("assembling-machine-3", {{"steel-chassis-large", 1}, {"stack-inserter", 2}, {"laser",2}, {"gold-plate", 16}, {"controller-mk2", 1}}, 1, "crafting", 8)

-- inserters
replace_recipe_ingredients("burner-inserter", {{"tin-plate", 2}, {"copper-motor", 1}, {"tin-rod", 3}}, 1, "crafting", 1)
replace_recipe_ingredients("inserter", {{"electronic-circuit",1}, {"iron-plate", 2}, {"iron-motor", 1}, {"iron-stick", 4}}, 1, "crafting", 1)
replace_recipe_ingredients("long-handed-inserter", {{"inserter", 1}, {"iron-stick", 4}, {"iron-rivet",1}, {"iron-motor", 1}}, 1, "crafting", 1)
replace_recipe_ingredients("fast-inserter", {{"electronic-circuit", 1}, {"steel-plate",2}, {"steel-motor", 1}, {"steel-rod",4}}, 1, "crafting", 1)
replace_recipe_ingredients("filter-inserter", {{"fast-inserter", 1}, {"sensor", 1}}, 1, "crafting", 1)
replace_recipe_ingredients("stack-inserter", {{"advanced-circuit",1}, {"titanium-plate", 2}, {"steel-motor", 2}, {"titanium-rod", 8}}, 1, "crafting", 2)
replace_recipe_ingredients("stack-filter-inserter", {{"stack-inserter", 1}, {"sensor", 1}}, 1, "crafting", 2)

-- electric age
replace_recipe_ingredients("lab", {{"iron-chassis-large",1}, {"computer-mk1",1}, {"iron-motor",4}, {"glass-plate",12}})
replace_recipe_ingredients("pipe", {{"iron-tube",1}, {"iron-rivet",1}})
replace_recipe_ingredients("pipe-to-ground", {{"pipe",10}}, 2)
replace_recipe_ingredients("small-lamp", {{"iron-plate", 4}, {"copper-cable", 4}, {"electronic-circuit", 1}, {"glass-plate",4}})
replace_recipe_ingredients("small-electric-pole", {{"wood-beam", 1}, {"copper-cable", 2}})
replace_recipe_ingredients("medium-electric-pole", {{"steel-beam", 2}, {"copper-cable", 4}})
replace_recipe_ingredients("big-electric-pole", {{"steel-beam", 8}, {"steel-cable", 6}, {"copper-cable-heavy", 4}}, 1, "crafting", 1)
replace_recipe_ingredients("substation", {{"steel-chassis-small", 3}, {"junction-box", 1}, {"steel-cable", 8}, {"advanced-circuit",1}})
replace_recipe_ingredients("stone-wall", {{"stone-brick", 5}}, 1, "crafting", 1)
replace_recipe_ingredients("gate", {{"stone-wall", 2}, {"iron-plate-heavy", 2}, {"iron-motor", 1}, {"electronic-circuit", 1}}, 1, "crafting", 1)
replace_recipe_ingredients("concrete", {{type = "item", name = "stone-gravel", amount = 5}, {type = "item", name = "iron-stick", amount = 5}, {type = "fluid", name = "water", amount = 100}}, 10, "crafting-with-fluid", 4)
replace_recipe_ingredients("refined-concrete", {{type = "item", name = "concrete", amount = 20}, {type = "item", name = "sulfur", amount = 1}, {type = "item", name = "steel-rod", amount = 10}, {type = "fluid", name = "water", amount = 100}}, 10, "crafting-with-fluid", 8)
replace_recipe_ingredients("offshore-pump", {{"pipe", 4}, {"iron-motor", 1}, {"iron-chassis-small", 1}}, 1, "crafting", 1)
replace_recipe_ingredients("solar-panel", {{"glass-plate", 9}, {"carbon-powder", 18}, {"steel-plate", 9}, {"steel-chassis-small",1}}, 1, "crafting", 2)
replace_recipe_ingredients("accumulator", {{"steel-chassis-small", 1}, {"advanced-battery", 3}, {"steel-ring", 9}, {"controller-mk2",1}, {"junction-box",1}}, 1, "crafting", 5)
replace_recipe_ingredients("pump", {{"pipe", 2}, {"iron-motor", 2}, {"electronic-circuit", 1}, {"iron-piston",2}}, 1, "crafting", 1)
replace_recipe_ingredients("storage-tank", {{"pipe", 4}, {"iron-plate-heavy", 40}, {"iron-stick", 20}}, 1, "crafting", 3)
replace_recipe_ingredients("boiler", {{"iron-chassis-small", 2}, {"pipe", 4}, {"stone-brick", 40}}, 1, "crafting", 3)
replace_recipe_ingredients("steam-engine", {{"iron-chassis-large", 1}, {"iron-piston", 16}, {"iron-gear-wheel", 16}, {"iron-plate", 40}}, 1, "crafting", 5)
replace_recipe_ingredients("pumpjack", {{"iron-chassis-small", 2}, {"iron-piston", 4}, {"iron-plate", 8}, {"iron-stick", 20}}, 1, "crafting", 3)

-- rail and vehicles
replace_recipe_ingredients("rail", {{"iron-ingot", 2}, {"wood-beam",1}, {"stone-gravel", 1}}, 1, "crafting", 1)
replace_recipe_ingredients("rail-signal", {{"glass-plate", 1}, {"copper-cable",3}, {"electronic-circuit", 1}, {"iron-plate",1}}, 1, "crafting", 1)
replace_recipe_ingredients("rail-chain-signal", {{"glass-plate", 1}, {"copper-cable",2}, {"electronic-circuit", 1}, {"iron-plate",1}}, 1, "crafting", 1)
replace_recipe_ingredients("train-stop", {{"rail-signal", 1}, {"controller-mk1", 1}, {"iron-chassis-small", 1}, {"iron-stick", 16}}, 1, "crafting", 1)
replace_recipe_ingredients("locomotive", {{"engine-unit", 3}, {"boiler", 1}, {"iron-plate",12}, {"controller-mk1",1}})
replace_recipe_ingredients("cargo-wagon", {{"iron-plate-heavy", 12}, {"iron-beam", 4}, {"iron-gear-wheel",8}})
replace_recipe_ingredients("fluid-wagon", {{"iron-plate-heavy", 20}, {"iron-beam", 6}, {"iron-gear-wheel",8}})
replace_recipe_ingredients("artillery-wagon", {{"chromium-gear-wheel",8},{"artillery-turret",1},{"advanced-engine-unit",4}})
replace_recipe_ingredients("car", {{"engine-unit", 1}, {"iron-chassis-small", 3}, {"rubber-natural",40}, {"iron-plate",4}, {"iron-gear-wheel",6}, {"glass-plate",6}})
replace_recipe_ingredients("tank", {{"engine-unit", 4}, {"titanium-chassis-large", 1}, {"steel-tube",10}, {"steel-gear-wheel",20}, {"rubber-vulcanised",100}, {"steel-plate-heavy",40}})

-- petrochem
replace_recipe_ingredients("oil-refinery", {{"iron-chassis-large", 3}, {"pipe", 30}, {"controller-mk1", 3}, {"concrete",200}})
replace_recipe_ingredients("chemical-plant", {{"iron-chassis-large", 1}, {"pipe", 6}, {"controller-mk1", 1}, {"concrete",40}})
replace_recipe_ingredients("empty-barrel", {{"steel-plate", 4}})

-- electronics - note circuit crafting times are overridden by items-misc later
-- circuits are found in items-misc
replace_recipe_ingredients("red-wire", {{"copper-cable", 10}}, 10)
replace_recipe_ingredients("green-wire", {{"copper-cable", 10}}, 10)
replace_recipe_ingredients("arithmetic-combinator", {{"iron-chassis-small",1}, {"electronic-circuit",2}, {"copper-cable",2}})
replace_recipe_ingredients("decider-combinator", {{"iron-chassis-small",1}, {"electronic-circuit",2}, {"copper-cable",2}})
replace_recipe_ingredients("constant-combinator", {{"iron-chassis-small",1}, {"electronic-circuit",1}, {"copper-cable",2}})
replace_recipe_ingredients("power-switch", {{"iron-chassis-small",1}, {"electronic-circuit",1}, {"copper-cable",4}})
replace_recipe_ingredients("programmable-speaker", {{"iron-chassis-small",1}, {"tin-plate",2}, {"electronic-circuit",2}, {"copper-cable",3}})

-- storage & logistics
replace_recipe_ingredients("wooden-chest", {{"wood-beam",6}}, 1, "crafting", 2)
replace_recipe_ingredients("iron-chest", {{"iron-chassis-small",1}, {"iron-plate",2}}, 1, "crafting", 2)
replace_recipe_ingredients("steel-chest", {{"steel-chassis-small",1}, {"steel-plate",2}}, 1, "crafting", 2)
replace_recipe_ingredients("logistic-chest-active-provider", {{"steel-chest",1}, {"steel-plate", 2}, {"advanced-circuit",1}, {"steel-motor", 1}}, 1, "crafting", 3)
replace_recipe_ingredients("logistic-chest-passive-provider", {{"steel-chest",1}, {"steel-plate", 2}, {"advanced-circuit",1}, {"steel-motor", 1}}, 1, "crafting", 3)
replace_recipe_ingredients("logistic-chest-storage", {{"steel-chest",1}, {"steel-plate", 2}, {"advanced-circuit",1}, {"steel-motor", 1}}, 1, "crafting", 3)
replace_recipe_ingredients("logistic-chest-requester", {{"steel-chest",1}, {"steel-plate", 2}, {"processing-unit",1}, {"steel-motor", 1}}, 1, "crafting", 3)
replace_recipe_ingredients("logistic-chest-buffer", {{"steel-chest",1}, {"steel-plate", 2}, {"processing-unit",1}, {"steel-motor", 1}}, 1, "crafting", 3)

-- replace_recipe_ingredients("flying-robot-frame", {{name = "plastic-fluid", type = "fluid", amount = 50}, {name = "iron-motor", type = "item", amount = 2}, {name = "battery", type = "item", amount = 5}})
replace_recipe_ingredients("construction-robot", {{"iron-chassis-small",1}, {"controller-mk1",1}, {"iron-motor",1}, {"battery",1}})
replace_recipe_ingredients("logistic-robot", {{"steel-chassis-small",1}, {"controller-mk2",1}, {"steel-motor",1}, {"battery",1}})
replace_recipe_ingredients("roboport", {{"controller-mk2",4}, {"steel-chassis-large",1}, {"steel-motor",4}, {"accumulator",1}})

-- late game
replace_recipe_ingredients("beacon", {{"steel-chassis-large",1}, {"field-effector",4}, {"computer-mk3",1}, {"junction-box",1}, {"steel-plate",32}})
replace_recipe_ingredients("steam-turbine", {{"controller-mk3",1}, {"steel-chassis-large", 1}, {"chromium-piston", 16}, {"chromium-gear-wheel",32},{"chromium-plate-heavy",16},{"junction-box",1}})
replace_recipe_ingredients("centrifuge", {{"computer-mk2",1}, {"steel-chassis-large", 3}, {"refined-concrete", 80}, {"lead-plate",24}, {"steel-gear-wheel",32}})
replace_recipe_ingredients("nuclear-reactor", {{"computer-mk3",8}, {"refined-concrete",320}, {"titanium-bulkhead", 120}, {"steel-plate-heavy",80}, {"heat-pipe",8}})
replace_recipe_ingredients("heat-exchanger", {{"steel-chassis-small", 2}, {"pipe", 4}, {"heat-pipe",4}, {"steel-plate", 40}})
replace_recipe_ingredients("heat-pipe", {{"copper-rod",12}, {"steel-rod", 12}, {"steel-cable",12}})
replace_recipe_ingredients("low-density-structure", {{"steel-plate", 2}, {"copper-foil", 40}, {"carbon-plate", 2}}, 1, "advanced-crafting", 20)
replace_recipe_ingredients("uranium-fuel-cell", {{"steel-tube", 10}, {"lead-tube",10}, {"uranium-235", 1}, {"uranium-238", 19}}, 10, "centrifuging", 20)
replace_recipe_ingredients("rocket-control-unit", {{"controller-mk2", 1}, {"sensor",8}, {"gyroscope",1}}, 1, "advanced-crafting", 8)
replace_recipe_ingredients("rocket-silo", {{"steel-bulkhead",240},{"refined-concrete",480},{"computer-mk3",8},{"junction-box",16},{"steel-tube",80},{"steel-rivet",80}})

-- military
replace_recipe_ingredients("pistol", {{"wood-beam",1}, {"copper-tube", 1}, {"tin-plate", 2}})
replace_recipe_ingredients("shotgun", {{"wood-beam", 1}, {"copper-tube", 4}, {"copper-gear-wheel", 2}, {"copper-rod",2}})
replace_recipe_ingredients("submachine-gun", {{"iron-tube", 3}, {"iron-gear-wheel", 2}, {"iron-stick",4}, {"iron-plate",2}})
replace_recipe_ingredients("combat-shotgun", {{"bronze-tube", 6}, {"steel-plate", 10}, {"steel-gear-wheel", 5}, {"bronze-rod",4}})
replace_recipe_ingredients("rocket-launcher", {{"steel-tube", 10}, {"steel-gear-wheel", 5}, {"steel-rod",3}, {"electronic-circuit",1}})
replace_recipe_ingredients("flamethrower", {{"steel-tube", 8}, {"steel-gear-wheel", 3}, {"steel-rod",4}})
replace_recipe_ingredients("flamethrower-ammo", {{name = "steel-tube", amount = 1, type = "item"}, {name = "light-oil", amount = 80, type = "fluid"}})
replace_recipe_ingredients("gun-turret", {{"iron-chassis-small",2},{"iron-motor",2},{"iron-tube",12},{"iron-plate-heavy",10}})
replace_recipe_ingredients("laser-turret", {{"steel-chassis-small",2},{"laser",4},{"advanced-battery",1},{"controller-mk2",1},{"junction-box",1},{"gyroscope",4}})
replace_recipe_ingredients("flamethrower-turret", {{"steel-chassis-large",1},{"pump",1},{"steel-tube",10},{"steel-plate-heavy",12},{"gyroscope",4}})
replace_recipe_ingredients("artillery-turret", {{"titanium-chassis-large",1},{"refined-concrete",100},{"chromium-tube",40},{"titanium-beam",8},{"computer-mk3",1},{"gyroscope",24}})
replace_recipe_ingredients("radar", {{"iron-chassis-large",1},{"iron-plate",16},{"magnetron",4},{"controller-mk1",1}})
replace_recipe_ingredients("rocket", {{"steel-tube",4}, {"steel-rod",4}, {"electronic-circuit",1}, {"explosives",1}})
replace_recipe_ingredients("atomic-bomb", {{"explosive-rocket",1}, {"uranium-235",30}, {"rocket-control-unit",1}, {"lead-tube",10}, {"steel-tube",10}})
replace_recipe_ingredients("explosives", {{name = "water", type = "fluid", amount = 50}, {name = "carbon-powder", type = "item", amount = 1}, {name = "sand", type = "item", amount = 1}, {name = "sulfur", type = "item", amount = 1}}, 2)
replace_recipe_ingredients("cliff-explosives", {{"explosives",10},{"iron-tube",1}})
replace_recipe_ingredients("land-mine", {{"steel-plate-heavy",4}, {"explosives",2}}, 4)
replace_recipe_ingredients("cannon-shell", {{"steel-tube",5}, {"explosives",1}, {"steel-rivet",4}})
replace_recipe_ingredients("explosive-cannon-shell", {{"cannon-shell",1}, {"explosives",2}})
replace_recipe_ingredients("artillery-shell", {{"steel-tube",20}, {"steel-plate",8}, {"steel-rivet",8}, {"explosives",16}, {"magnetron",1} })
replace_recipe_ingredients("grenade", {{"steel-tube",1}, {"carbon-gravel",5}, {"sand",5}})
replace_recipe_ingredients("cluster-grenade", {{"grenade",7}, {"steel-rod",6}, {"explosives",3}})
replace_recipe_ingredients("poison-capsule", {{name = "glass-ingot", amount = 1, type = "item"}, {name = "sulfuric-acid", amount = 200, type = "fluid"}, {name = "lead-pure", amount = 3, type = "item"}, {name = "explosives", amount = 1, type = "item"}}, 1, "crafting-with-fluid", 4)
replace_recipe_ingredients("slowdown-capsule", {{name = "glass-ingot", amount = 1, type = "item"}, {name = "plastic-fluid", amount = 100, type = "fluid"}, {name = "rubber-natural", amount = 20, type = "item"}, {name = "explosives", amount = 1, type = "item"}}, 1, "crafting-with-fluid", 4)
replace_recipe_ingredients("defender-capsule", {{"steel-chassis-small",1}, {"steel-magazine",5}, {"controller-mk1",1}, {"steel-tube",3}})
replace_recipe_ingredients("distractor-capsule", {{"steel-chassis-small",3}, {"laser",3}, {"controller-mk2",3}, {"steel-plate-heavy",12}})
replace_recipe_ingredients("destroyer-capsule", {{"steel-chassis-small",10}, {"charged-advanced-battery",5}, {"steel-ring",10}, {"steel-plate-heavy",30}, {"controller-mk3",5}})
replace_recipe_ingredients("artillery-targeting-remote", {{"processing-unit",1}})

-- equipment
replace_recipe_ingredients("light-armor", {{"tin-plate", 20}})
replace_recipe_ingredients("heavy-armor", {{"bronze-chassis-small",1},{"bronze-plate-heavy",20}})
replace_recipe_ingredients("modular-armor", {{"iron-chassis-small",1},{"iron-plate-heavy",20},{"rubber-natural",20},{"iron-motor",8},{"controller-mk1",1}})
replace_recipe_ingredients("power-armor", {{"steel-chassis-small",1},{"steel-plate-heavy",40},{"rubber-vulcanised",40},{"steel-motor",8},{"controller-mk2",1}})
replace_recipe_ingredients("power-armor-mk2", {{"titanium-chassis-small",2},{"titanium-plate-heavy",80},{"carbon-plate",80}, {"steel-motor",24},{"computer-mk3",1}})
replace_recipe_ingredients("night-vision-equipment", {{"iron-tube",2},{"sensor",2},{"controller-mk1",1},{"rubber-natural",2}})
replace_recipe_ingredients("belt-immunity-equipment", {{"iron-ingot",2},{"iron-gear-wheel",2},{"iron-ball",2}})
replace_recipe_ingredients("solar-panel-equipment", {{"solar-panel",1}, {"steel-rivet", 4}, {"electronic-circuit",1}})
replace_recipe_ingredients("energy-shield-equipment", {{"steel-chassis-small",1},{"field-effector",3},{"steel-ring",3},{"controller-mk2",1}})
replace_recipe_ingredients("energy-shield-mk2-equipment", {{"titanium-chassis-small",1},{"field-effector",12},{"steel-ring",12},{"computer-mk3",1}})
replace_recipe_ingredients("battery-equipment", {{"iron-tube",4},{"iron-plate",4},{"battery",4},{"electronic-circuit",3}})
replace_recipe_ingredients("battery-mk2-equipment", {{"chromium-tube",5},{"carbon-plate",8},{"advanced-battery",5},{"processing-unit",3}})
replace_recipe_ingredients("exoskeleton-equipment", {{"steel-motor",24},{"steel-rod",32},{"controller-mk2",1},{"carbon-plate",16}})
replace_recipe_ingredients("personal-roboport-equipment", {{"iron-chassis-small",1},{"iron-tube",10},{"iron-gear-wheel",4},{"controller-mk1",4}})
replace_recipe_ingredients("personal-roboport-mk2-equipment", {{"steel-chassis-small",2},{"chromium-tube",20},{"chromium-gear-wheel",8},{"computer-mk3",1}})
replace_recipe_ingredients("personal-laser-defense-equipment", {{"laser",4}, {"steel-chassis-small", 1}, {"sensor", 4}, {"computer-mk3",1},{"carbon-plate",8}})
replace_recipe_ingredients("discharge-defense-equipment", {{"advanced-battery",3}, {"steel-chassis-small", 1}, {"steel-ring",6}, {"computer-mk3",1},{"carbon-plate",8}})
replace_recipe_ingredients("fusion-reactor-equipment", {{"computer-mk3",3}, {"refined-concrete", 50}, {"chromium-plate-heavy",25}, {"titanium-plate-heavy",25}, {"lead-plate", 25}})

-- fluids
replace_recipe_ingredients("sulfuric-acid", {{name = "water", type = "fluid", amount = 100}, {name = "sulfur", type = "item", amount = 5}})
data.raw.recipe["sulfuric-acid"].results = {{ name = "sulfuric-acid", amount = 50, type = "fluid" }}
data.raw.recipe["sulfuric-acid"].show_amount_in_title = false
data.raw.recipe["sulfuric-acid"].always_show_products = true
replace_recipe_ingredients("rocket-fuel", {{name = "solid-fuel-yellow", amount = 10, type = "item"},{name = "iron-powder", amount = 1, type = "item"}}, 1)

-- misc
replace_recipe_ingredients("landfill", {{"stone-gravel",10},{"sand",10}}, 1, "crafting", 4)


------------------------------------------------------------------------------------------------------------------------------------------------------

-- upgrades to vanilla armour

data:extend {
    {
        type = "equipment-grid",
        name = "bronze-equipment-grid",
        equipment_categories = {"armor"},
        height = 4,
        width = 2
    },
}
data.raw.armor["heavy-armor"].equipment_grid = "bronze-equipment-grid"
data.raw.armor["heavy-armor"].stack_size = 1
data.raw.armor["light-armor"].inventory_size_bonus = 10
data.raw.armor["heavy-armor"].inventory_size_bonus = 20
data.raw.armor["modular-armor"].inventory_size_bonus = 30
data.raw.armor["power-armor"].inventory_size_bonus = 40
data.raw.armor["power-armor-mk2"].inventory_size_bonus = 50

------------------------------------------------------------------------------------------------------------------------------------------------------

-- uranium fuel cell reprocessing

replace_recipe_ingredients("nuclear-fuel-reprocessing", {{"used-up-uranium-fuel-cell", 1}}, 1, "centrifuging", 2)
data.raw.recipe["nuclear-fuel-reprocessing"].result = nil
data.raw.recipe["nuclear-fuel-reprocessing"].result_count = nil
data.raw.recipe["nuclear-fuel-reprocessing"].results = {
	{ name = "uranium-238", amount_min = 1, amount_max = 2 },
	{ name = "lead-scrap", amount = 1, probability = 0.5 },
	{ name = "steel-scrap", amount = 1, probability = 0.5 },
}


------------------------------------------------------------------------------------------------------------------------------------------------------

-- cosmetic/organisational tweaks

change_item_order("assembling-machine-1","aa")
change_item_order("assembling-machine-2","ab")
change_item_order("assembling-machine-3","ac")
change_item_order("beacon", "z")
change_item_order("construction-robot","b")
change_item_order("electric-mining-drill", "b")
change_item_order("empty-barrel", "aardvark-e")
change_item_order("explosives", "zzzzz")
change_item_order("lab", "bb")
change_item_order("logistic-robot","c")
change_item_order("nuclear-fuel","zzzzzzzzz")
change_item_order("offshore-pump", "b[pipe]-c[aardvark]")
change_item_order("pumpjack", "c")
change_item_order("roboport","d")
change_item_order("rocket-control-unit","bb")
change_item_order("rocket-fuel","zzzzzzzz")
change_item_order("rocket-silo", "zz")
change_item_order("storage-tank","b[fluid]-b[storage-tank]")
change_item_order("gun-turret","b[turret]-b")
change_item_order("laser-turret","b[turret]-c")
change_item_order("artillery-turret","b[turret]-z")
change_item_order("stone-wall","a")
change_item_order("gate","c")
change_item_order("radar","z")

change_item_subgroup("assembling-machine-1", "deadlock-machines-misc")
change_item_subgroup("assembling-machine-2", "deadlock-machines-misc")
change_item_subgroup("assembling-machine-3", "deadlock-machines-misc")
change_item_subgroup("battery", "intermediate-product")
change_item_subgroup("beacon", "production-machine")
change_item_subgroup("construction-robot","deadlock-robots")
change_item_subgroup("explosives", "ammo")
change_item_subgroup("lab", "deadlock-machines-misc")
change_item_subgroup("logistic-robot","deadlock-robots")
change_item_subgroup("offshore-pump", "energy-pipe-distribution")
change_item_subgroup("pumpjack", "deadlock-machines-resources")
change_item_subgroup("roboport","deadlock-robots")
change_item_subgroup("rocket-silo", "production-machine")
change_item_subgroup("solid-fuel", "fluid-recipes")
change_item_subgroup("rocket-fuel", "fluid-recipes")
change_item_subgroup("nuclear-fuel", "fluid-recipes")
change_item_subgroup("rocket-control-unit", "space-products")
-- change_item_subgroup("sulfur", "fluid-recipes")
change_item_subgroup("stone-wall", "deadlock-walls")
change_item_subgroup("gate", "deadlock-walls")
change_item_subgroup("radar", "deadlock-walls")

change_recipe_category("battery", "crafting-with-fluid")
change_recipe_category("flamethrower-ammo", "crafting-with-fluid")
change_recipe_category("engine-unit", "crafting")
change_recipe_category("rocket-fuel", "advanced-crafting")
change_recipe_category("nuclear-fuel", "advanced-crafting")

change_recipe_subgroup("uranium-processing", "uranium-processing")
change_recipe_subgroup("uranium-fuel-cell", "uranium-processing")
change_recipe_subgroup("nuclear-fuel-reprocessing", "uranium-processing")
change_recipe_subgroup("kovarex-enrichment-process", "uranium-processing")
change_recipe_subgroup("rocket-fuel", "uranium-processing")
change_recipe_subgroup("nuclear-fuel", "uranium-processing")

data.raw.recipe["explosives"].order = "z"
data.raw.recipe["explosives"].subgroup = "fluid-recipes-2"
data.raw.recipe["nuclear-fuel"].icon = nil
data.raw.recipe["nuclear-fuel"].icon_size = nil

data.raw.capsule["raw-fish"].order = "z"
data.raw.capsule["raw-fish"].subgroup = "wood"
data.raw.recipe["uranium-processing"].order = "q[uranium-processing]"
data.raw["item-with-entity-data"]["car"].icon = get_icon_path("car")
data.raw["item-with-entity-data"]["car"].icon_size = DIR.icon_size
data.raw["item-with-entity-data"]["car"].order = "z-car"
data.raw["item-with-entity-data"]["car"].subgroup = "deadlock-robots"
data.raw["item-with-entity-data"]["tank"].icon = get_icon_path("tank")
data.raw["item-with-entity-data"]["tank"].icon_size = DIR.icon_size
data.raw["item-with-entity-data"]["tank"].order = "z-tank"
data.raw["item-with-entity-data"]["tank"].subgroup = "deadlock-robots"

replace_item_icon("assembling-machine-1","assembler1")
replace_item_icon("assembling-machine-2","assembler2")
replace_item_icon("assembling-machine-3","assembler3")
replace_item_icon("battery")
replace_item_icon("burner-inserter","grey-inserter")
replace_item_icon("construction-robot")
replace_item_icon("crude-oil")
replace_item_icon("empty-barrel")
replace_item_icon("fast-inserter","blue-inserter")
replace_item_icon("filter-inserter","purple-inserter")
replace_item_icon("heavy-oil")
replace_item_icon("inserter","yellow-inserter")
replace_item_icon("lab","vanilla-lab")
replace_item_icon("light-oil")
replace_item_icon("logistic-robot")
replace_item_icon("long-handed-inserter","red-inserter")
replace_item_icon("low-density-structure")
replace_item_icon("lubricant")
replace_item_icon("nuclear-reactor")
replace_item_icon("nuclear-fuel")
replace_item_icon("petroleum-gas")
replace_item_icon("pumpjack")
replace_item_icon("radar")
replace_item_icon("roboport")
replace_item_icon("rocket-control-unit")
replace_item_icon("rocket-fuel")
replace_item_icon("rocket-silo")
replace_item_icon("small-lamp")
replace_item_icon("solid-fuel")
replace_item_icon("stack-filter-inserter","white-inserter")
replace_item_icon("stack-inserter","green-inserter")
replace_item_icon("sulfuric-acid")
replace_item_icon("water")
replace_item_icon("gun-turret","autogun-turret")
replace_item_icon("laser-turret")
replace_item_icon("iron-chest")
replace_item_icon("steel-chest")
replace_item_icon("red-wire")
replace_item_icon("green-wire")
replace_item_icon("logistic-chest-storage")
replace_item_icon("logistic-chest-passive-provider")
replace_item_icon("logistic-chest-active-provider")
replace_item_icon("logistic-chest-buffer")
replace_item_icon("logistic-chest-requester")

if not mods["aai-industry"] and not mods["Dectorio"] then
	replace_item_icon("gate")
	replace_item_icon("stone-wall")
end

for k,v in pairs(DIR.science_packs) do
    replace_item_icon(v)
end

data.raw.item["nuclear-fuel"].stack_size = 3
data.raw.item["uranium-fuel-cell"].stack_size = 10
data.raw.item["train-stop"].stack_size = 50

------------------------------------------------------------------------------------------------------------------------------------------------------

-- wooden chest gets its fuel value back

data.raw.item["wooden-chest"].fuel_value = "5MJ"
data.raw.item["wooden-chest"].fuel_category = "chemical"

------------------------------------------------------------------------------------------------------------------------------------------------------
