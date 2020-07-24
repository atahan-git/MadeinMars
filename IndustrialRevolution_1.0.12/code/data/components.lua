------------------------------------------------------------------------------------------------------------------------------------------------------

-- COMPONENTS

return {
	["ore"] = {
		type = "item",
		order = "aa",
		materials = {"wood","rubber","stone","carbon","copper","tin","iron","gold","uranium"},
		start = true,
		skip_scan = true,
		speed = DIR.machines_to_belt_ratio,
		scale_speed = true,
		category = "natural",
		belt_variations = 6,
		subgroup_exceptions = {
			wood = "wood",
			rubber = "wood",
		},
		tabgroup = "deadlock-processing",
		productivity = true,
	},
	["gravel"] = {
		type = "item",
		order = "b",
		materials = {"stone","carbon","copper","tin","iron","gold","uranium"},
		skip_scan = true,
		speed = DIR.machines_to_belt_ratio/2,
		scale_speed = true,
		category = "grinding",
		made_from = {
			ore = { amount = 1, result = 1 },
		},
		belt_variations = 6,
		-- subgroup = "ore-products",
		tabgroup = "deadlock-processing",
		subgroup_exceptions = {
			uranium = "ore",
		},
		productivity = true,
		tile_walking_speed = {
			stone = 1.1,
			carbon = 1.1,
			copper = 1.1,
			tin = 1.1,
			iron = 1.1,
			gold = 1.1,
		},
	},
	["pure"] = {
		type = "item",
		order = "c",
		materials = {"copper","tin","iron","gold","lead","titanium","chromium","platinum","uranium"},
		skip_scan = true,
		speed = DIR.machines_to_belt_ratio/2,
		scale_speed = true,
		category = "washing",
		made_from = {
			gravel = { amount = 1, result = 1 },
		},
		made_from_secondary = {
			water = DIR.washing_base,
		},
		bonus_results = {
			copper = { 
				["ruby-gem"]=0.01,
			},
			tin = {
				["lead-pure"]=0.2,
				["sapphire-gem"]=0.01,
			},
			iron = {
				["titanium-pure"]=0.2,
				["chromium-pure"]=0.1,
			},
			gold = {
				["platinum-pure"]=0.02,
				["ruby-gem"]=0.03,
			},
		},
		duplication = {
			prefix = "advanced-",
			replace_ingredients = { water = "diluted-sulfuric-acid" },
			replace_multiplier = { water = 1 },
			result_probability = 0.4,
			byproduct_probability_multiplier = 2,
			time_multiplier = 1,
			options = {
				iron = {"titanium-pure","chromium-pure"},
			},
		},
		category_results_secondary = {
			["washing"] = { name = "dirty-water", type = "fluid", amount = DIR.washing_base, catalyst_amount = DIR.washing_base },
			["advanced-washing"] = { name = "dirty-water", type = "fluid", amount = DIR.washing_base, catalyst_amount = DIR.washing_base},
		},
		belt_variations = 6,
		-- subgroup = "ore-products",
		tabgroup = "deadlock-processing",
		subgroup_exceptions = {
			uranium = "ore",
			wood = "fluid-recipes-2",
		},
		crafting_exceptions = {
			wood = "smelting",
		},
		order_exceptions = {
			wood = "zzz",
		},
		start_exceptions = {
			wood = true,
		},
		speed_exceptions = {
			wood = DIR.machines_to_belt_ratio,
		},
		productivity = true,
		block_material_info = true,
	},
	["powder"] = {
		type = "item",
		order = "d",
		materials = {"stone","carbon","copper","tin","iron","gold","lead","titanium"},
		skip_scan = true,
		speed = DIR.machines_to_belt_ratio/2,
		scale_speed = true,
		category = "grinding",
		made_from = {
			pure = { amount = 1, result = 1 },
		},
		bonus_results = {
			carbon = {
				["diamond-gem"]=0.0005,
			},
		},
		made_from_exceptions = {
			stone = {
				gravel = {amount = 1, result = 1}, -- sand
			},
			carbon = {
				gravel = {amount = 1, result = 1},
			},
		},
		tabgroup = "deadlock-processing",
		productivity = true,
	},
	["scrap"] = {
		type = "item",
		order = "e4",
		materials = {"copper","tin","bronze","iron","gold","steel","lead","titanium","duranium","glass"}, -- order of this list defines icon variants, ugh
		start = true,
		skip_scan = true,
		category = "scrapping",
		belt_variations = 4,
		tabgroup = "deadlock-processing",
		stack_size = 50,
		block_tertiary_ingredients = true,
	},
	["fluid"] = {
		type = "fluid",
		order = "z",
		materials = {"plastic","chromium"},
		speed = DIR.machines_to_belt_ratio/2,
		category = "chemistry",
		skip_scan = true,
		made_from = {
			ore = { amount = 1, result = 10 },
		},
		made_from_exceptions = {
			chromium = {
				pure = {amount = 1, result = 100},
			},
			plastic = {
				fluid = {amount = 0, result = 20}, -- skipped
			},
		},
		made_from_tertiary = {
			plastic = { ["carbon-powder"] = 2, ["petroleum-gas"] = 20 },
			chromium = { ["sulfuric-acid"] = 100 },
		},
		speed_exceptions = {
			plastic = 1,
		},
		tabgroup = "deadlock-processing",
		subgroup = "fluid-recipes-2",
		item_subgroup = "fluid",
		productivity = true,
		ignore_hardness = true,
		block_material_info = true,
	},
	["ingot"] = {
		type = "item",
		order = "e3",
		materials = {"stone","glass","plastic","copper","tin","bronze","iron","gold","steel","titanium","lead","duranium"},
		start = true,
		speed = DIR.machines_to_belt_ratio,
		scale_speed = true,
		category = "smelting",
		made_from = {
			ore = { amount = 12, result = 12 },
			gravel = { amount = 10, result = 12 },
			pure = { amount = 8, result = 12 },
			powder = { amount = 6, result = 12 },
			scrap = { amount = 12/DIR.scrap_divider, result = 12 },
		},
		made_from_exceptions = {
			stone = {
				ore = { amount = 24, result = 12 },
			},
			plastic = {
				fluid = { amount = 20, result = 2 },
			},
			bronze = {
				ingot = { amount = 0, result = 12 }, -- alloy
				scrap = { amount = 12/DIR.scrap_divider, result = 12 },
			},
			glass = {
				ingot = { amount = 0, result = 12 }, -- alloy
				scrap = { amount = 12/DIR.scrap_divider, result = 12 },
			},
			steel = {
				ingot = { amount = 0, result = 12 }, -- alloy
				scrap = { amount = 12/DIR.scrap_divider, result = 12 },
			},
			duranium = {
				ingot = { amount = 0, result = 12 }, -- alloy
				scrap = { amount = 12/DIR.scrap_divider, result = 12 },
			},
			titanium = {
				pure = { amount = 8, result = 12 },
				powder = { amount = 6, result = 12 },
				scrap = { amount = 12/DIR.scrap_divider, result = 12 },
			},
			lead = {
				pure = { amount = 8, result = 12 },
				powder = { amount = 6, result = 12 },
				scrap = { amount = 12/DIR.scrap_divider, result = 12 },
			},
		},
		made_from_tertiary = {
			bronze = { ["copper-ingot"] = 8, ["tin-ingot"] = 4 }, -- alloy
			glass = { ["sand"] = 10, ["tin-gravel"] = 2 }, -- alloy
			steel = { ["iron-ingot"] = 12, ["carbon-powder"] = 2 }, -- alloy
			duranium = { ["titanium-ingot"] = 9, ["gold-ingot"] = 3 }, -- alloy
		},
		crafting_exceptions = {
			plastic = "crafting-with-fluid",
		},
		speed_exceptions = {
			plastic = 0.5,
			glass = DIR.machines_to_belt_ratio/2,
			bronze = DIR.machines_to_belt_ratio/2,
			steel = DIR.machines_to_belt_ratio/2,
			duranium = DIR.machines_to_belt_ratio/2,
		},
		-- source_emissions_multiplier = {
			-- pure = 0.5,
			-- powder = 0.5,
		-- },
		variants = {
			tin = 2,
			lead = 2,
			gold = 3,
			duranium = 2,
		},
		tabgroup = "deadlock-processing",
		subgroup_exceptions = {
			copper = "ingots-1",
			glass = "non-handcrafting",
			gold = "ingots-2",
			iron = "ingots-2",
			plastic = "non-handcrafting",
			stone = "non-handcrafting",
			tin = "ingots-1",
			duranium = "uranium-processing",
		},
		item_subgroup = "ingots",
		productivity = true,
		tile_walking_speed = {
			stone = 1.25,
		},
	},
	["plate"] = {
		type = "item",
		order = "f",
		start = true,
		derivative = true,
		materials = {"carbon","glass","chromium","copper","tin","bronze","iron","gold","steel","titanium","lead","duranium","rubber"},
		made_from = {
			ingot = { amount = 1, result = 1 },
		},
		made_from_exceptions = {
			carbon = {
				powder = {amount = 3, result = 1},
			},
			chromium = {
				fluid = {amount = 100, result = 5},
			},
			rubber = {
				ore = {amount = 1, result = 4},
			},
			glass = {
				ingot = { amount = 6, result = 12 },
			},
		},
		made_from_tertiary = {
			carbon = { ["plastic-fluid"] = 30, ["steam"] = 20 },
			chromium = { ["steel-plate"] = 5 },
			duranium = { ["sulfuric-acid"] = 10 },
			glass = { ["tin-ingot"] = 3 },
		},
		crafting_exceptions = {
			carbon = "chemistry",
			glass = "smelting",
			rubber = "grinding",
			chromium = "electroplating",
		},
		speed_exceptions = {
			glass = DIR.machines_to_belt_ratio * 6,
			rubber = DIR.machines_to_belt_ratio/2,
			chromium = 2.5,
		},
		scrapping_exclusions = {
			glass = { ["tin-ingot"] = 3 },
		},
		recipe_locale_name_exceptions = {
			glass = "recipe-name.glass-float",
		},
		variants = {
			duranium = 2,
			titanium = 2,
			chromium = 2,
		},
	},
	["plate-heavy"] = {
		type = "item",
		order = "g",
		materials = {"chromium","copper","tin","bronze","iron","steel","titanium","duranium","rubber"},
		start = true,
		derivative = true,
		made_from = {
			plate = { amount = 3, result = 1 },
		},
		made_from_exceptions = {
			rubber = {
				plate = {amount = 4, result = 4},
			},
		},
		made_from_tertiary = {
			rubber = { ["sulfur"] = 1 },
		},
		made_from_secondary = {
			rivet = 2,
		},
		crafting_exceptions = {
			carbon = "chemistry",
			glass = "smelting",
			rubber = "smelting",
		},
		speed_exceptions = {
			rubber = DIR.machines_to_belt_ratio/2,
		},
	},
	["beam"] = {
		type = "item",
		order = "i",
		start = true,
		speed = 1,
		materials = {"wood","copper","bronze","iron","steel","titanium","duranium"},
		made_from = {
			plate = { amount = 2, result = 1 },
		},
		made_from_tertiary = {
			copper = { ["wood-beam"] = 1, ["copper-rivet"] = 1 },
			bronze = { ["wood-beam"] = 1, ["bronze-rivet"] = 1 },
			duranium = { ["sulfuric-acid"] = 30 },
		},
		made_from_exceptions = {
			wood = {
				ore = {amount = 1, result = 2},
			},
			iron = {
				ingot = {amount = 3, result = 1},
			},
			steel = {
				ingot = {amount = 3, result = 1},
			},
			titanium = {
				ingot = {amount = 3, result = 1},
			},
			duranium = {
				ingot = {amount = 3, result = 1},
			},
		},
	},
	["tube"] = {
		type = "item",
		order = "j",
		materials = {"chromium","copper","tin","bronze","iron","steel","titanium","lead","duranium"},
		start = true,
		derivative = true,
		made_from = {
			plate = { amount = 2, result = 1 },
		},
		made_from_exceptions = {
			chromium = {
				fluid = {amount = 100, result = 5},
			},
		},
		made_from_tertiary = {
			chromium = { ["steel-tube"] = 5 },
		},
		crafting_exceptions = {
			chromium = "electroplating",
		},
		speed_exceptions = {
			chromium = 2.5,
		},
	},
	["gear-wheel"] = {
		type = "item",
		order = "k",
		materials = {"chromium","copper","tin","bronze","iron","steel","titanium"},
		start = true,
		derivative = true,
		made_from = {
			plate = { amount = 1, result = 1 },
		},
		made_from_exceptions = {
			chromium = {
				fluid = {amount = 100, result = 5},
			},
		},
		made_from_tertiary = {
			chromium = { ["steel-gear-wheel"] = 5 },
		},
		crafting_exceptions = {
			chromium = "electroplating",
		},
		variants = {
			steel = 2,
			titanium = 2,
			chromium = 2,
		},
		speed = 0.5,
		speed_exceptions = {
			chromium = 1.25,
		},
	},
	["rod"] = {
		type = "item",
		order = "h",
		materials = {"chromium","copper","tin","bronze","iron","gold","steel","titanium","duranium"},
		start = true,
		derivative = true,
		made_from = {
			ingot = { amount = 1, result = 2 },
		},
		made_from_exceptions = {
			chromium = {
				fluid = {amount = 50, result = 5},
			},
			wood = {
				beam = {amount = 1, result = 4},
			},
		},
		made_from_tertiary = {
			chromium = { ["steel-rod"] = 5 },
			duranium = { ["sulfuric-acid"] = 10 },
		},
		crafting_exceptions = {
			glass = "smelting",
			chromium = "electroplating",
		},
		subgroup_exceptions = {
			wood = "beam",
		},
		variants = {
			steel = 2,
			titanium = 2,
			duranium = 3,
			chromium = 2,
		},
		speed = 1,
		speed_exceptions = {
			chromium = 2.5,
		},
		stack_size = 200,
	},
	["cable"] = {
		type = "item",
		order = "l",
		start = false,
		derivative = false,
		materials = {"copper","gold","steel","glass","lead"},
		made_from = {
			rod = { amount = 1, result = 1 },
		},
		made_from_exceptions = {
			glass = {
				ingot = {amount = 1, result = 2}, -- fibre-optic cable
			},
			lead = {
				ingot = {amount = 3, result = 12}, -- solder
			},
		},
		made_from_tertiary = {
			glass = { ["plastic-fluid"] = 20, ["chromic-acid"] = 20 },
			lead = { ["tin-ingot"] = 9 },
		},
		crafting_exceptions = {
			glass = "chemistry",
			lead = "smelting",
		},
		speed_exceptions = {
			glass = 1,
			lead = DIR.machines_to_belt_ratio * 6,
		},
		speed = 0.5,
		stack_size = 200,
		recipe_locale_name_exceptions = {
			lead = "item-name.solder",
		},
	},
	["cable-heavy"] = {
		type = "item",
		order = "d",
		speed = 2,
		materials = {"copper","glass"},
		made_from = {
			cable = { amount = 8, result = 1 },
		},
		made_from_exceptions = {
			glass = {
				cable = { amount = 8, result = 1 },
			},
		},
		made_from_tertiary = {
			copper = { ["rubber-natural"] = 1 },
			glass = { ["plastic-bar"] = 2 },
		},
		tabgroup = "intermediate-products",
		subgroup = "intermediate-product",
		ignore_hardness = true,
		stack_size = 50,
	},
	["rivet"] = {
		type = "item",
		order = "n",
		materials = {"chromium","copper","tin","bronze","iron","steel","titanium","duranium"},
		start = true,
		derivative = true,
		made_from = {
			rod = { amount = 1, result = 1 },
		},
		made_from_exceptions = {
			chromium = {
				fluid = {amount = 50, result = 5},
			},
		},
		made_from_tertiary = {
			chromium = { ["steel-rivet"] = 5 },
		},
		crafting_exceptions = {
			chromium = "electroplating",
		},
		speed = 0.5,
		speed_exceptions = {
			chromium = 1.25,
		},
		stack_size = 200,
	},
	["ball"] = {
		type = "item",
		order = "o",
		materials = {"chromium","copper","tin","bronze","iron","steel","titanium"},
		start = true,
		derivative = true,
		made_from = {
			ingot = { amount = 1, result = 2 },
		},
		made_from_exceptions = {
			chromium = {
				fluid = {amount = 50, result = 5},
			},
		},
		made_from_tertiary = {
			chromium = { ["steel-ball"] = 5 },
		},
		crafting_exceptions = {
			chromium = "electroplating",
		},
		speed_exceptions = {
			chromium = 2.5,
		},
		stack_size = 200,
	},
	["foil"] = {
		type = "item",
		order = "p",
		materials = {"copper","gold"},
		start = false,
		derivative = false,
		made_from = {
			ingot = { amount = 1, result = 2 },
		},
		subgroup = "cable",
		stack_size = 200,
	},
	["repair-tool"] = {
		type = "repair-tool",
		order = "aaa",
		materials = {"copper","bronze","iron","steel","titanium"},
		start = true,
		derivative = true,
		skip_scan = true,
		speed = 0.5,
		made_from = {
			rod = { amount = 10, result = 1 },
		},
		made_from_tertiary = {
			copper = { ["wood-beam"] = 1 },
			bronze = { ["wood-beam"] = 1 },
			iron = { ["rubber-natural"] = 1 },
			steel = { ["rubber-natural"] = 1 },
			titanium = { ["rubber-natural"] = 1 },
		},
		tabgroup = "intermediate-products",
		subgroup = "intermediates-2",
		ignore_hardness = true,
		block_material_info = true,
	},
	["piston"] = {
		type = "item",
		order = "aab",
		materials = {"copper","iron","steel","chromium"},
		start = true,
		speed = 1,
		derivative = true,
		made_from = {
			tube = { amount = 1, result = 1 },
		},
		made_from_tertiary = {
			copper = { ["copper-rod"] = 1, ["copper-plate"] = 2, },
			bronze = { ["bronze-rod"] = 1, ["bronze-plate"] = 2, },
			iron = { ["iron-stick"] = 1, ["iron-plate"] = 2, ["iron-rivet"] = 1},
			steel = { ["steel-rod"] = 2, ["steel-plate"] = 2, ["steel-rivet"] = 2 },
			chromium = { ["titanium-rod"] = 2, ["chromium-plate"] = 2, ["chromium-rivet"] = 2 },
		},
		tabgroup = "intermediate-products",
		subgroup = "intermediates-2",
		ignore_hardness = true,
		block_material_info = true,
	},
	["chassis-small"] = {
		type = "item",
		order = "aac",
		materials = {"copper","bronze","iron","steel","titanium"},
		speed = 2,
		start = true,
		derivative = true,
		made_from = {
			rod = { amount = 12, result = 1 },
		},
		made_from_secondary = {
			rivet = 4,
		},
		subgroup = "chassis",
		tabgroup = "intermediate-products",
		ignore_hardness = true,
		block_material_info = true,
	},
	["chassis-large"] = {
		type = "item",
		order = "aad",
		materials = {"copper","bronze","iron","steel","titanium"},
		start = true,
		speed = 4,
		derivative = true,
		made_from = {
			["beam"] = { amount = 12, result = 1 },
		},
		made_from_secondary = {
			rivet = 8,
		},
		subgroup = "chassis",
		tabgroup = "intermediate-products",
		ignore_hardness = true,
		block_material_info = true,
		stack_size = 50,
	},
	["bulkhead"] = {
		type = "item",
		order = "a",
		materials = {"steel","titanium","duranium"},
		start = false,
		speed = 4,
		derivative = true,
		made_from = {
			["plate-heavy"] = { amount = 4, result = 1 },
		},
		made_from_tertiary = {
			steel = { ["steel-beam"] = 4 },
			titanium = { ["titanium-beam"] = 4, ["lead-plate"] = 4 },
			duranium = { ["duranium-beam"] = 4, ["carbon-plate"] = 4 },
		},
		subgroup = "space-products",
		tabgroup = "intermediate-products",
		ignore_hardness = true,
		block_material_info = true,
		stack_size = 50,
	},
	["motor"] = {
		type = "item",
		order = "b",
		materials = {"copper","iron","steel"},
		start = true,
		speed = 2,
		speed_exceptions = {
			copper = 1,
			steel = 3,
		},
		derivative = false,
		made_from = {
			tube = { amount = 1, result = 1 },
		},
		made_from_tertiary = {
			copper = { ["copper-gear-wheel"] = 1, ["copper-ball"] = 2 },
			iron = { ["iron-gear-wheel"] = 2, ["copper-cable"] = 2, ["iron-ball"] = 2 },
			steel = { ["steel-gear-wheel"] = 4, ["copper-cable"] = 4, ["steel-ball"] = 4, ["lubricant"] = 10 },
		},
		tabgroup = "intermediate-products",
		subgroup = "intermediate-product",
		ignore_hardness = true,
		block_material_info = true,
	},
	["engine"] = {
		type = "item",
		order = "c",
		materials = {"iron","steel","chromium"},
		start = true,
		speed = 6,
		speed_exceptions = {
			steel = 8,
			chromium = 12,
		},
		derivative = false,
		made_from = {
			piston = { amount = 4, result = 1 },
		},
		made_from_secondary = {
			["gear-wheel"] = 2,
			ball = 2,
			tube = 4,
		},
		made_from_exceptions = {
			steel = {
				piston = { amount = 6, result = 1 },
			},
			chromium = {
				piston = { amount = 8, result = 1 },
			},
		},
		made_from_tertiary = {
			steel = { ["steel-tube"] = 6, ["steel-gear-wheel"] = 4, ["steel-ball"] = 4, ["lubricant"] = 40 },
			chromium = { ["chromium-tube"] = 8, ["chromium-gear-wheel"] = 6, ["chromium-ball"] = 6, ["lubricant"] = 80 },
		},
		tabgroup = "intermediate-products",
		subgroup = "intermediate-product",
		ignore_hardness = true,
		block_material_info = true,
		stack_size = 50,
	},
	["ring"] = {
		type = "item",
		order = "kb",
		materials = {"steel","duranium"},
		start = false,
		derivative = false,
		made_from = {
			tube = { amount = 4, result = 1 },
		},
		made_from_tertiary = {
			steel = { ["steel-cable"] = 8, ["copper-cable"] = 32 },
			duranium = { ["duranium-rod"] = 16, ["gold-cable"] = 16 },
		},
		speed = 8,
		subgroup = "space-products",
		tabgroup = "intermediate-products",
		block_material_info = true,
	},
	["gem"] = {
		type = "item",
		order = "w",
		materials = {"diamond","ruby","sapphire"},
		skip_scan = true,
		subgroup = "gravel",
		tabgroup = "deadlock-processing",
	},
	["magazine"] = {
		type = "ammo",
		order = "a",
		materials = {"copper","iron","steel","titanium","uranium"},
		start = true,
		skip_scan = true,
		made_from = {
			rod = { amount = 4, result = 1 },
		},
		made_from_exceptions = {
			steel = {
				rod = { amount = 6, result = 1 },
			},
			titanium = {
				rod = { amount = 10, result = 1 },
			},
			uranium = {
				gravel = {amount = 1, result = 1},
			},
		},
		made_from_tertiary = {
			copper = { ["copper-plate"] = 2 },
			iron = { ["iron-plate"] = 2 },
			steel = { ["steel-plate"] = 2 },
			titanium = { ["titanium-plate"] = 3 },
			uranium = { ["titanium-magazine"] = 1, ["lead-plate"] = 2 },		
		},
		subgroup = "ammo",
		tabgroup = "military",
		ignore_hardness = true,
		block_material_info = true,
	},
	["cartridge"] = {
		type = "ammo",
		order = "b",
		materials = {"copper","iron","steel","titanium","uranium"},
		start = true,
		skip_scan = true,
		made_from = {
			ball = { amount = 4, result = 1 },
		},
		made_from_exceptions = {
			steel = {
				ball = { amount = 6, result = 1 },
			},
			titanium = {
				ball = { amount = 10, result = 1 },
			},
			uranium = {
				gravel = {amount = 1, result = 1},
			},
		},
		made_from_tertiary = {
			copper = { ["copper-plate"] = 2 },
			iron = { ["iron-plate"] = 2 },
			steel = { ["steel-plate"] = 2 },
			titanium = { ["titanium-plate"] = 3 },
			uranium = { ["titanium-cartridge"] = 1, ["lead-plate"] = 2 },		
		},
		subgroup = "ammo",
		tabgroup = "military",
		ignore_hardness = true,
		block_material_info = true,
	},
	["belt"] = {
		type = "ammo",
		order = "c",
		materials = {"titanium"},
		start = true,
		skip_scan = true,
		made_from = {
			magazine = { amount = 10, result = 1 },
		},
		made_from_tertiary = {
			titanium = { ["steel-cable"] = 4 },
		},
		subgroup = "ammo",
		tabgroup = "military",
		ignore_hardness = true,
		block_material_info = true,
		stack_size = 20,
		speed = 5,
	},
}

------------------------------------------------------------------------------------------------------------------------------------------------------
