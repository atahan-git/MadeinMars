------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-maths")
require("code.functions.functions-items")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- global variables container

DIR = {}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- graphics paths and sizes

DIR.icon_size = 64
DIR.icon_mipmaps = 4
DIR.icon_path = "__IndustrialRevolution__/graphics/icons"
DIR.sprites_path = "__IndustrialRevolution__/graphics/entities"
DIR.terrain_path = "__IndustrialRevolution__/graphics/terrain"
DIR.projectiles_path = "__IndustrialRevolution__/graphics/entities/projectiles"
DIR.equipment_path = "__IndustrialRevolution__/graphics/equipment"
DIR.sound_path = "__IndustrialRevolution__/sound"
DIR.base_sound_path = "__base__/sound"
DIR.core_sound_path = "__core__/sound"
DIR.anim_speed = 0.75

------------------------------------------------------------------------------------------------------------------------------------------------------

-- crafting, speed ratios and belt speeds

DIR.default_belt_speed = 15 -- yellow belt items/s
DIR.base_energy_unit = 4 -- MJ
DIR.belt_energy_value = DIR.base_energy_unit * DIR.default_belt_speed -- MW
DIR.machines_per_belt = 48 -- how many furnace-type machines should saturate a yellow belt at speed 1
DIR.machines_to_belt_ratio = DIR.machines_per_belt / DIR.default_belt_speed -- with default numbers, 3.2
DIR.clusters_per_energy_belt = 10 -- how many arrays of machines_per_belt uses up one "energy belt" at crafting speed 1
DIR.washing_base = 100 -- how much fluid is used in washing recipes
DIR.washing_ratio = 0.95 -- proportion of water reclaimed from dirty water filtering
DIR.washers_to_cleaners = 4 -- number of washers dealt with by 1 cleaner
DIR.max_tiers = 5 -- highest material tier
DIR.basic_drill_speed = 2 / DIR.machines_to_belt_ratio
DIR.base_research_time = 30 -- the basic multiplier of research time in seconds
DIR.scrap_minimum_rate = 0.5 -- the proportion of scrap that is guaranteed to be returned - average amount is halfway between this and 100%
DIR.scrap_divider = 3 -- how many ingots is 1 scrap worth
DIR.scrap_loot_penalty = 0.5 -- proportion of an item's scrap value that destroyed buildings drop
DIR.default_crafting_time = 1

------------------------------------------------------------------------------------------------------------------------------------------------------

-- misc constants

DIR.display_frame = "display_selector"
DIR.oil_flame_colour = {r=1,g=0.75,b=0.25}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- caches

DIR.ancestors = {}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- logging

DIR.log_level = {
	debug = 1,
	warning = 2,
	error = 3,
}

DIR.log = {
	[DIR.log_level.debug] = false,
	[DIR.log_level.warning] = true,
	[DIR.log_level.error] = true,
}

DIR.log_prefix = {
	[DIR.log_level.debug] = "debug: ",
	[DIR.log_level.warning] = "WARNING: ",
	[DIR.log_level.error] = "ERROR: ",
}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- GENERATED BASICS

-- named material hues - used by ammo and crafting tints

DIR.hues = {
	red = 1, -- we use 1 to mean red and 0 to mean greyscale
	orange = 1/12,
	yellow = 1/6,
	green = 1/3,
	lime = 5/12,
	cyan = 1/2,
	azure = 7/12,
	blue = 2/3,
	violet = 3/4,
	magenta = 5/6,
	greyscale = 0,
}

-- materials

DIR.materials = require("code.data.materials")

-- components

DIR.components = require("code.data.components")

-- some items in the DIR name-space directly correspond to vanilla items which use some other name
-- others are just really hard to remember e.g. sand rather than "stone powder"
-- all functions which deal with item/recipe names should check these first

DIR.name_overrides = {
	["carbon-ore"] = "coal",
	["chromium-engine"] = "advanced-engine-unit",
	["chromium-fluid"] = "chromic-acid",
	["iron-engine"] = "engine-unit",
	["iron-rod"] = "iron-stick",
	["lead-cable"] = "solder",
	["plastic-ingot"] = "plastic-bar",
	["rubber-ore"] = "rubber-wood",
	["rubber-plate"] = "rubber-natural",
	["rubber-plate-heavy"] = "rubber-vulcanised",
	["steel-engine"] = "electric-engine-unit",
	["stone-ingot"] = "stone-brick",
	["stone-ore"] = "stone",
	["stone-powder"] = "sand",
	["uranium-gravel"] = "uranium-238",
	["uranium-pure"] = "uranium-235",
	["wood-ore"] = "wood",
	["wood-pure"] = "charcoal",
	["wood-rod"] = "wood-stick",
}

-- belt variation exceptions

DIR.belt_variation_exclusions = {
	"wood",
	"rubber-wood",
	"uranium-235",
	"uranium-238",
}

-- use vanilla icons/variations for these components

DIR.vanilla_icon_override = {
	["uranium-ore"] = true,
}

-- sometimes crafting categories need to replace the base layer icon

DIR.category_icons = {
	["washing"] = "washing-background",
	["advanced-washing"] = "advanced-washing-background",
}

-- all known crafting categories - some also have a tier suffix e.g. smelting-2

DIR.crafting_categories = {
	"advanced-crafting",
	"advanced-washing",
	"charging",
	"chemistry",
	"cleaning",
	"crafting",
	"crafting-with-fluid",
	"electroplating",
	"endgame-crafting",
	"forestry",
	"grinding",
	"incinerating",
	"natural",
	"scrapping",
	"smelting",
	"voiding",
	"washing",
}

-- tiered crafting categories - crafting machines using these categories get a tier suffix per category

DIR.tiered_categories = {"smelting", "grinding"}

-- components that define the root of total-raw calculations

DIR.non_decomposition_components = {"ingot"}

-- fuel values for specific comp-mat combos - this is a multiplier of the material base fuel value

DIR.component_fuel_values = {
	["coal"] = { multiplier = 1 },
	["carbon-gravel"] = { multiplier = 1, emissions_multiplier = 0.75 },
	["wood"] = { multiplier = 1 },
	["wood-beam"] = { multiplier = 0.5 },
	["wood-stick"] = { multiplier = 0.125 },
	["rubber-wood"] = { multiplier = 1, emissions_multiplier = 1.25 },
}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- TECHNOLOGY

-- ignore lists for ancestors search - advanced techs which only provide recipes which are alternative routes to results that are already obtainable
-- these technologies are highly likely to induce circularity

DIR.ancestor_tech_ignore_list = {
	-- vanilla
	"advanced-oil-processing", "coal-liquefaction", "nuclear-fuel-reprocessing", "kovarex-enrichment-process",
	-- IR
	"deadlock-modules-1","deadlock-modules-2","deadlock-modules-3", "deadlock-washing-2",
	-- AAI Industry
	"fuel-processing",
}

-- recipe substrings that exclude a recipe from redundancy checks - must be escaped with %
-- these substrings also exclude recipes from being "providers" in the tech tree rebuild
DIR.redundant_recipe_exclusions = {
	"deadlock%-lamp",
	"deadlock%-packrecipe",
	"deadlock%-unpackrecipe",
	"deadlock%-stacks",
	"scrap%-",
}

-- ingredients which are freely available under any circumstance and don't ever need prerequisite chaining

DIR.ancestor_ingredient_ignore_list = {"water", "crude-oil", "iron-ore", "copper-ore", "tin-ore", "gold-ore", "stone", "coal", "wood", "rubber-wood", "wood-sapling", "rubber-sapling"}

-- results with probabilities are usually ignored in ancestor search - except these ones, because they are the only source of an item

DIR.tech_search_allowed_probability_components = {
	["titanium-pure"] = true,
	["lead-pure"] = true,
	["chromium-pure"] = true,
	["platinum-pure"] = true,
	["diamond-gem"] = true,
	["ruby-gem"] = true,
	["sapphire-gem"] = true,
	["uranium-235"] = true,
	["uranium-238"] = true,
}

-- main DIR tech backbone

DIR.age_time_multiplier = 2

DIR.technologies = {
	["deadlock-bronze-age"] = {
		machines = {},
		unlocks = {
			["bronze"] = {"ingot-from-ingot","ingot-from-scrap","derivatives"},
		},
		additional_unlocks = {"bronze-beam","bronze-chest"},
		icon = "bronze-milestone",
		packs = {"automation"},
		prerequisites = {"logistics","automation"},
		time_multiplier = DIR.age_time_multiplier
	},
	["deadlock-furnaces-1"] = {
		machines = {"bronze-furnace"},
		unlocks = {},
		additional_unlocks = {},
		icon = "bronze-furnace",
		packs = {"automation"},
		prerequisites = {"deadlock-bronze-age"},
	},
	["deadlock-grinding-1"] = {
		machines = {"bronze-grinder"},
		unlocks = {
			["stone"] = {"gravel","powder"},
			["carbon"] = {"gravel","powder"},
			["copper"] = {"ingot-from-gravel","gravel"},
			["tin"] = {"ingot-from-gravel","gravel"},
			["glass"] = {"ingot-from-ingot","ingot-from-scrap","plate"}, -- not derivatives because of optics
			["rubber"] = {"plate"},
		},
		additional_unlocks = {"bronze-display"},
		icons = {"bronze-grinder"},
		packs = {"automation"},
		prerequisites = {"deadlock-furnaces-1","automation"},
	},
	["deadlock-iron-age"] = {
		machines = {},
		unlocks = {
			["iron"] = {"ingot-from-ore","ingot-from-scrap","derivatives"},
		},
		additional_unlocks = {"iron-beam","iron-chest","iron-display"},
		icon = "iron-milestone",
		packs = {"automation","logistics"},
		prerequisites = {"deadlock-furnaces-1"},
		time_multiplier = DIR.age_time_multiplier
	},
	["deadlock-steam-power"] = {
		machines = {"steam-engine","boiler"},
		unlocks = {},
		additional_unlocks = {"small-electric-pole","big-wooden-pole","iron-motor","pipe","pipe-to-ground","offshore-pump","copper-cable","copper-cable-heavy"},
		icon = "steam-power",
		packs = {"automation","logistics"},
		prerequisites = {"deadlock-iron-age"},
	},
	["deadlock-mining-1"] = {
		machines = {"electric-mining-drill"},
		unlocks = {
			["gold"] = {"ingot-from-ore","ingot-from-scrap","derivatives"},
		},
		additional_unlocks = {"gold-chest","gold-display"},
		icon = "electric-mining-drill",
		packs = {"automation","logistics"},
		prerequisites = {"deadlock-steam-power"},
	},
	["deadlock-furnaces-2"] = {
		machines = {"iron-furnace","iron-incinerator"},
		unlocks = {},
		additional_unlocks = {},
		icon = "iron-furnace",
		packs = {"automation","logistics"},
		prerequisites = {"deadlock-steam-power"},
	},
	["deadlock-grinding-2"] = {
		machines = {"iron-grinder"},
		unlocks = {
			["iron"] = {"ingot-from-gravel","gravel"},
			["gold"] = {"ingot-from-gravel","gravel"},
		},
		icons = {"iron-grinder"},
		packs = {"automation","logistics"},
		prerequisites = {"deadlock-mining-1","deadlock-furnaces-2"},
	},
	["deadlock-steel-age"] = {
		machines = {},
		unlocks = {
			["steel"] = {"ingot-from-ingot","ingot-from-scrap","derivatives","motor","beam"},
		},
		additional_unlocks = {"gyroscope","steel-chest"},
		icon = "steel-milestone",
		packs = {"automation","logistics","chemical"},
		prerequisites = {"deadlock-furnaces-2"},
		time_multiplier = DIR.age_time_multiplier
	},
	["deadlock-washing-1"] = {
		machines = {"steel-washer","steel-cleaner"},
		unlocks = {
			["copper"] = {"pure","ingot-from-pure"},
			["tin"] = {"pure","ingot-from-pure"},
			["iron"] = {"pure","ingot-from-pure"},
			["gold"] = {"pure","ingot-from-pure"},
			["lead"] = {"ingot-from-pure","ingot-from-scrap","derivatives","cable"},
		},
		additional_unlocks = {"dirty-water-cleaning"},
		icons = {"steel-washer","water"},
		packs = {"automation","logistics","chemical"},
		prerequisites = {"deadlock-steel-age","deadlock-grinding-2"},
	},
	["deadlock-furnaces-3"] = {
		machines = {"steel-age-furnace"},
		unlocks = {
			["titanium"] = {"ingot-from-pure","ingot-from-scrap","derivatives"},
		},
		additional_unlocks = {"titanium-beam","graphite-electrode"},
		icon = "steel-age-furnace",
		packs = {"automation","logistics","chemical"},
		prerequisites = {"deadlock-washing-1"},
	},
	["deadlock-chrome-age"] = {
		machines = {"steel-electroplater"},
		unlocks = {
			["chromium"] = {"fluid","plate","plate-heavy","gear-wheel","rod","tube","rivet","ball","piston"},
		},
		additional_unlocks = {},
		icon = "chromium-milestone",
		packs = {"automation","logistics","chemical"},
		prerequisites = {"deadlock-furnaces-3"},
		time_multiplier = DIR.age_time_multiplier
	},
	["deadlock-grinding-3"] = {
		machines = {"steel-grinder"},
		unlocks = {
			["copper"] = {"powder","ingot-from-powder"},
			["tin"] = {"powder","ingot-from-powder"},
			["iron"] = {"powder","ingot-from-powder"},
			["gold"] = {"powder","ingot-from-powder"},
			["lead"] = {"powder","ingot-from-powder"},
			["titanium"] = {"powder","ingot-from-powder"},
			["platinum"] = {"powder"},
			["chromium"] = {"powder"},
		},
		additional_unlocks = {},
		icons = {"steel-grinder"},
		packs = {"automation","logistics","chemical","production"},
		prerequisites = {"deadlock-furnaces-3"},
	},
	["deadlock-washing-2"] = {
		machines = {},
		unlocks = {},
		additional_unlocks = {"advanced-copper-pure","advanced-tin-pure","advanced-iron-pure","advanced-gold-pure","diluted-sulfuric-acid"},
		icons = {"steel-washer","sulfuric-acid"},
		packs = {"automation","logistics","chemical","production"},
		prerequisites = {"deadlock-grinding-3"},
	},
	["deadlock-mining-2"] = {
		machines = {"chrome-drill"},
		unlocks = {},
		icon = "chrome-drill",
		packs = {"automation","logistics","chemical","production"},
		prerequisites = {"deadlock-chrome-age"},
	},
	["deadlock-future-age"] = {
		machines = {},
		unlocks = {
			["duranium"] = {"ingot-from-ingot","ingot-from-scrap","derivatives"},
		},
		additional_unlocks = {"duranium-beam","duranium-ring"},
		icon = "duranium-milestone",
		packs = {"automation","logistics","chemical","production","utility","space"},
		prerequisites = {},
		time_multiplier = DIR.age_time_multiplier
	}
}

-- science packs

DIR.science_packs = {
	automation = "automation-science-pack",
	logistics = "logistic-science-pack",
	military = "military-science-pack",
	chemical = "chemical-science-pack",
	production = "production-science-pack",
	utility = "utility-science-pack",
	space = "space-science-pack"
}

-- "seeds" for tech tree tiers - anything with any of the following as an ancestor prereq also inherits the pack requirement
-- native DIR tech backbone also seeds

DIR.tech_classes = {
	logistics = {
		"chemical-science-pack",
		"deadlock-normal-inserter-capacity-bonus-1",
		"weapon-shooting-speed-2",
		"physical-projectile-damage-2",
	},
	military = {
		"military-2",
		"weapon-shooting-speed-3",
		"physical-projectile-damage-3",
	},
	chemical = {
		"weapon-shooting-speed-4",
		"physical-projectile-damage-4",
		"research-speed-3",
		"braking-force-3",
		"worker-robots-speed-2",
		"deadlock-carbon-fibre",
	},
	production = {
		"advanced-oil-processing",
		"automation-3",
		"deadlock-modules-3",
		"effect-transmission",
		"inserter-capacity-bonus-3",
		"logistics-3",
		"rocket-fuel",
		"uranium-processing",
		"kovarex-enrichment-process",
		"braking-force-6",
		"worker-robots-storage-3",
		"worker-robots-speed-4",
		"deadlock-robot-battery-4",
	},
	utility = {
		"battery-mk2-equipment",
		"character-logistic-slots-4",
		"character-logistic-trash-slots-4",
		"effect-transmission",
		"energy-shield-mk2-equipment",
		"fusion-reactor-equipment",
		"logistic-system",
		"personal-laser-defense-equipment",
		"personal-roboport-mk2-equipment",
		"power-armor-mk2",
		"research-speed-5",
		"rocket-control-unit",
		"rocket-silo",
		"weapon-shooting-speed-5",
		"physical-projectile-damage-5",
		"energy-weapons-damage-4",
		"deadlock-photon-turret",
	},
	space = { -- all infinite techs also have space added automatically
		"deadlock-space-surveys-1",
		"mining-productivity-1",
	},
}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- MACHINES

DIR.machines = {
	-- mining & resources
	["burner-mining-drill"] = {ingredients = {{"copper-chassis-small",2}, {"tin-plate",8}, {"copper-piston",1}, {"copper-motor",1}}, enabled = true, subgroup = "deadlock-machines-resources", order = "a", time = 3},
	["electric-mining-drill"] = {ingredients = {{"iron-chassis-large",1}, {"iron-plate-heavy",16}, {"iron-piston",4}, {"iron-motor",4}, {"copper-cable-heavy",1}}, enabled = false, subgroup = "deadlock-machines-resources", order = "b", time = 5},
	["chrome-drill"] = {ingredients = {{"titanium-chassis-large",1}, {"chromium-plate-heavy",32}, {"gold-plate",32}, {"chromium-piston",8}, {"advanced-engine-unit",4}, {"diamond-gem",36}}, enabled = false, time = 12, subgroup = "deadlock-machines-resources", order = "c", stack_size = 10},
	["bronze-forestry"] = {ingredients = {{"bronze-rod",128},{"bronze-plate",12},{"glass-plate",48},{"stone-gravel",20}}, enabled = false, subgroup = "deadlock-machines-resources", order = "za"},
	["iron-forestry"] = {ingredients = {{"iron-stick",128},{"iron-plate",12},{"glass-plate",48},{"stone-gravel",40},{"small-lamp",8}}, enabled = false, subgroup = "deadlock-machines-resources", order = "zb"},
	["copper-incinerator"] = {ingredients = {{"copper-chassis-small",1}, {"copper-tube",2}}, enabled = true, subgroup = "deadlock-machines-resources", order = "zc", time = 2},
	["iron-incinerator"] = {ingredients = {{"iron-chassis-small",1}, {"iron-tube",2}, {"copper-cable",6}}, enabled = false, subgroup = "deadlock-machines-resources", order = "zd", time = 2},
	["copper-scrapper"] = {ingredients = {{"copper-chassis-large",1},{"copper-motor",2},{"copper-piston",2},{"tin-gear-wheel",4}}, enabled = false, subgroup = "deadlock-machines-resources", order = "ze"},
	["iron-scrapper"] = {ingredients = {{"iron-chassis-large",1},{"iron-motor",4},{"iron-piston",8},{"controller-mk1",1}}, enabled = false, subgroup = "deadlock-machines-resources", order = "zf"},
	-- furnaces, grinders, washers
	["stone-age-furnace"] = {ingredients = {{"stone",20},{"wood-beam",8}}, enabled = true, subgroup = "deadlock-machines-processing"},
	["bronze-furnace"] = {ingredients = {{"bronze-chassis-large",1},{"stone-brick",20}}, enabled = false, subgroup = "deadlock-machines-processing"},
	["iron-furnace"] = {ingredients = {{"iron-chassis-large",1},{"iron-plate-heavy",12},{"copper-cable",16},{"controller-mk1",1}}, enabled = false, subgroup = "deadlock-machines-processing"},
	["steel-age-furnace"] = {ingredients = {{"steel-chassis-large",1},{"steel-plate-heavy",36},{"graphite-electrode",3},{"controller-mk2",1},{"junction-box",1}}, enabled = false, subgroup = "deadlock-machines-processing", stack_size = 20},
	["bronze-grinder"] = {ingredients = {{"bronze-chassis-large",1},{"copper-motor",2},{"bronze-ingot",12},{"stone-brick",12}}, enabled = false, subgroup = "deadlock-machines-processing"},
	["iron-grinder"] = {ingredients = {{"iron-chassis-large",1},{"iron-piston",4},{"iron-ingot",16},{"controller-mk1",1}}, enabled = false, subgroup = "deadlock-machines-processing"},
	["steel-electroplater"] = {ingredients = {{"steel-chassis-large",1},{"copper-rod",8},{"junction-box",1},{"controller-mk2",1},{"gold-plate",16}}, enabled = false, subgroup = "deadlock-machines-misc", order = "ad"},
	["steel-grinder"] = {ingredients = {{"steel-chassis-large",1},{"steel-piston",4},{"steel-ingot",16},{"controller-mk2",1},{"gold-plate",16}}, enabled = false, subgroup = "deadlock-machines-processing"},
	["steel-washer"] = {ingredients = {{"steel-chassis-large",1},{"steel-motor",2},{"steel-piston",2},{"controller-mk2",1},{"gold-plate",16}}, enabled = false, subgroup = "deadlock-machines-processing"},
	["steel-cleaner"] = {ingredients = {{"steel-chassis-large",1},{"pump",1},{"steel-plate-heavy",8},{"controller-mk2",1},{"gold-plate",16}}, enabled = false, subgroup = "deadlock-machines-processing"},
	-- labs and misc
    ["watchtower"] = {ingredients = {{"stone",20},{"copper-gear-wheel",4},{"copper-motor",1},{"tin-plate",8}}, enabled = true, subgroup = "deadlock-walls", order = "x"},
	["copper-lab"] = {ingredients = {{"copper-chassis-large",1}, {"tin-plate",12}, {"copper-piston", 2}}, enabled = true, subgroup = "deadlock-machines-misc", order = "ba"},
	["quantum-lab"] = {ingredients = {{"quantum-ring",3}, {"computer-mk3",8}, {"glass-cable-heavy",8}, {"chromium-plate-heavy",80}, {"steel-chassis-large",2},{"junction-box",8}}, enabled = false, time = 20, subgroup = "deadlock-machines-misc", order = "bc", stack_size = 10},
	["iron-battery-charger"] =	{ingredients = {{"iron-chassis-small",2},{"copper-rod",2},{"copper-cable-heavy",1},{"electronic-circuit",1}}, enabled = false, time = 3, subgroup = "deadlock-machines-misc"},
	["iron-battery-discharger"] =  {ingredients = {{"iron-chassis-small",2},{"iron-motor",2},{"copper-cable-heavy",1},{"electronic-circuit",1}}, enabled = false, time = 3, subgroup = "deadlock-machines-misc"},
	["long-handed-burner-inserter"] = {ingredients = {{"burner-inserter",1},{"tin-rod",4},{"copper-motor",1}}, enabled = true, subgroup = "inserter", order = "a[burner-z]", time = 1},
	["big-wooden-pole"] = {ingredients = {{"wood",2},{"copper-cable-heavy",1},{"iron-plate",2}}, enabled = false, subgroup = "energy-pipe-distribution", order = "a[energy]-a[z]", time = 1},
	["bronze-display"] = {ingredients = {{"bronze-plate",4},{"rubber-natural",1}}, enabled = false, subgroup = "terrain", order = "za", time = 1},
	["iron-display"] = {ingredients = {{"iron-plate",4},{"rubber-natural",1}}, enabled = false, subgroup = "terrain", order = "zb", time = 1},
	["gold-display"] = {ingredients = {{"gold-plate",4},{"rubber-natural",1}}, enabled = false, subgroup = "terrain", order = "zc", time = 1},
	["steel-plate-wall"] = {ingredients = {{"steel-plate-heavy",5},{"stone-wall",1}}, enabled = false, subgroup = "deadlock-walls", order = "b", time = 4},
	-- storage
	["tin-chest"] = {ingredients = {{"tin-rod",6}, {"tin-plate",3}}, enabled = true, subgroup = "storage", order = "a[items]-b[aaaa]", time = 2},
	["bronze-chest"] = {ingredients = {{"bronze-chassis-small",1}, {"bronze-plate",2}}, enabled = false, subgroup = "storage", order = "a[items]-b[aaab]", time = 2},
	["gold-chest"] = {ingredients = {{"gold-plate",4}, {"iron-chassis-small",1}}, enabled = false, subgroup = "storage", order = "a[items]-b[zzz]", time = 2},
	["bronze-storage-depot"] = {ingredients = {{"bronze-chassis-large",1}, {"bronze-chest",10}, {"bronze-plate-heavy",12}}, enabled = false, subgroup = "storage", order = "a[items]-d"},
	["steel-storage-depot"] = {ingredients = {{"steel-chassis-large",1}, {"steel-chest",10}, {"steel-plate-heavy",12}}, enabled = false, subgroup = "storage", order = "a[items]-e"},
	["logistics-depot-active"] = {ingredients = {{"steel-storage-depot",1}, {"controller-mk2",1}, {"steel-motor",1}}, enabled = false, subgroup = "logistic-network"},
	["logistics-depot-passive"] = {ingredients = {{"steel-storage-depot",1}, {"controller-mk2",1}, {"steel-motor",1}}, enabled = false, subgroup = "logistic-network"},
	["logistics-depot-storage"] = {ingredients = {{"steel-storage-depot",1}, {"controller-mk2",1}, {"steel-motor",1}}, enabled = false, subgroup = "logistic-network"},
	["logistics-depot-buffer"] = {ingredients = {{"steel-storage-depot",1}, {"controller-mk3",1}, {"steel-motor",1}}, enabled = false, subgroup = "logistic-network"},
	["logistics-depot-requester"] = {ingredients = {{"steel-storage-depot",1}, {"controller-mk3",1}, {"steel-motor",1}}, enabled = false, subgroup = "logistic-network"},
	["small-tank"] = {ingredients = {{"iron-chassis-small",1}, {"iron-plate-heavy",4},{"pipe",2}}, enabled = false, subgroup = "storage", order = "b[fluid]-a[small-tank]", time = 2},
	["robotower"] = {ingredients = {{"steel-chassis-small",4},{"controller-mk2",2},{"steel-motor",2},{"battery",4}}, enabled = false, subgroup = "deadlock-robots", order = "cz"},
	["slow-filter-inserter"] = {ingredients = {{"inserter",1},{"sensor",1}}, enabled = false, subgroup = "inserter", order = "c[z]"},
	-- vanilla groups
	["solar-array"] = {ingredients = {{"solar-panel",4}, {"advanced-circuit",1}, {"junction-box",1}}, enabled = false, subgroup = "energy", order = "d[solar-panel]-b[solar-array]"},
	["flare-stack"] = {ingredients = {{"iron-chassis-small",1},{"pipe",8},{"copper-cable",2}}, enabled = false, subgroup = "production-machine", order = "d[z-flare-stack]"},
	["scattergun-turret"] = {ingredients = {{"copper-chassis-small",2},{"copper-motor",1},{"copper-plate",8},{"copper-tube",4}}, enabled = false, subgroup = "defensive-structure", order = "b[turret]-a", time = 3},
	["arc-turret"] = {ingredients = {{"steel-chassis-small",2},{"advanced-battery",1},{"controller-mk2",1},{"steel-ring",3},{"junction-box",1},{"gyroscope",4}}, enabled = false, subgroup = "defensive-structure", order = "b[turret]-d"},
	["minigun-turret"] = {ingredients = {{"titanium-chassis-large",1},{"titanium-chassis-small",1},{"gyroscope",6},{"titanium-plate-heavy",40},{"steel-tube",18}}, enabled = false, time = 8, subgroup = "defensive-structure", order = "b[turret]-e"},
	["photon-turret"] = {ingredients = {{"titanium-chassis-large",2},{"gyroscope",12},{"chromium-plate-heavy",80},{"advanced-battery",2},{"computer-mk3",1},{"junction-box",1}}, enabled = false, time = 12, subgroup = "defensive-structure", order = "b[turret]-f"},
	-- vehicles
	["monowheel"] = {ingredients = {{"copper-chassis-small",1}, {"copper-motor",1}, {"tin-gear-wheel",8}, {"tin-rod",16}, {"copper-tube",8}}, enabled = false, subgroup = "deadlock-robots", order = "z-a", type = "item-with-entity-data", stack_size = 1},
	["racing-monowheel"] = {ingredients = {{"steel-chassis-small",1}, {"advanced-engine-unit",1}, {"chromium-gear-wheel",16}, {"chromium-rod",32}, {"gyroscope",2}, {"gold-plate",4}}, enabled = false, subgroup = "deadlock-robots", order = "z-ab", type = "item-with-entity-data", stack_size = 1},
}

-- machines which have layered/tinted icons
DIR.icon_masks = {
	["logistics-depot-active"] = {base = "logistics-depot-base", mask = "logistics-depot-mask", tint = {0.4, 0.0, 0.6, 0.5}},
	["logistics-depot-passive"] = {base = "logistics-depot-base", mask = "logistics-depot-mask", tint = {0.6, 0.0, 0.0, 0.5}},
	["logistics-depot-requester"] = {base = "logistics-depot-base", mask = "logistics-depot-mask", tint = {0.0, 0.3, 0.6, 0.5}},
	["logistics-depot-storage"] = {base = "logistics-depot-base", mask = "logistics-depot-mask", tint = {0.6, 0.6, 0.0, 0.5}},
	["logistics-depot-buffer"] = {base = "logistics-depot-base", mask = "logistics-depot-mask", tint = {0.0, 0.6, 0.0, 0.5}},
}

-- machine indicator colours

DIR.indicator_colours = {
	iron = {r=0.5,g=0.9,b=1.0},
	steel = {r=1.0,g=0.85,b=0.5},
}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- AMMO

DIR.ammo = {
	range = {
		magazine = 18,
		cartridge = 15,
		belt = 24,
	},
	turret_speed = {
		magazine = seconds_to_ticks(0.1),
		cartridge = seconds_to_ticks(1),
		belt = seconds_to_ticks(0.05),
	},
	size = {
		magazine = 10,
		cartridge = 10,
		belt = 100,
	},
	reload = {
		cartridge = seconds_to_ticks(0.5),
		belt = seconds_to_ticks(1),
	},
}

DIR.projectile_speed = {
	bullet = 1.5,
	shotgun = 1,
}

DIR.photon = {
	torpedo_cost = 20, -- MJ
	buffer = 20, -- MJ
	charge_time = 2, -- seconds, for one torpedo
}

DIR.laser_colours = {
	red = {index = 0, rgb = { 1, 1/2, 1/2, 1} },
	orange = {index = 1, rgb = { 1, 3/4, 1/2, 1} },
	yellow = {index = 2, rgb = { 1, 1, 1/2, 1} },
	lime = {index = 3, rgb = { 3/4, 1, 1/2, 1} },
	green = {index = 4, rgb = { 1/2, 1, 1/2, 1} },
	teal = {index = 5, rgb = { 1/2, 1, 3/4, 1} },
	cyan = {index = 6, rgb = { 1/2, 1, 1, 1} },
	azure = {index = 7, rgb = { 1/2, 3/4, 1, 1} },
	blue = {index = 8, rgb = { 1/2, 1/2, 1, 1} },
	violet = {index = 9, rgb = { 3/4, 1/2, 1, 1} },
	magenta = {index = 10, rgb = { 1, 1/2, 1, 1} },
	pink = {index = 11, rgb = { 1, 1/2, 3/4, 1} },
}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- basic list for productivity modules - this is added to by the base item generation process (for all components with productivity flag set)

DIR.productivity_limitation = {
	"basic-oil-processing",
	"coal-liquefaction",
	"heavy-oil-cracking",
	"light-oil-cracking",
	"advanced-heavy-oil-cracking",
	"advanced-light-oil-cracking",
	"solid-fuel-from-light-oil",
	"solid-fuel-from-heavy-oil",
	"solid-fuel-from-petroleum-gas",
	"lubricant",
	"sulfur",
	"uranium-processing",
	"rocket-fuel",
	"rocket-part",
	"explosives",
	"kovarex-enrichment-process",
	"sulfuric-acid",
	"low-density-structure",
	"rocket-control-unit",
	"automation-science-pack",
	"logistic-science-pack",
	"chemical-science-pack",
	"military-science-pack",
	"production-science-pack",
	"utility-science-pack",
	"glass-plate",
	"carbon-plate",
	"rubber-natural",
	"solder",
}

------------------------------------------------------------------------------------------------------------------------------------------------------
