------------------------------------------------------------------------------------------------------------------------------------------------------

-- FORESTRY

------------------------------------------------------------------------------------------------------------------------------------------------------

-- template

local forestry = {
	name = "bronze-forestry",
	type = "assembling-machine",
	crafting_speed = 1,
	icon = get_icon_path("bronze-forestry", DIR.icon_size),
	placeable_by = {item = "bronze-forestry", count = 1},
	fast_replaceable_group = "forestry",
	minable = {
		mining_time = 1,
		result = "bronze-forestry",
	},
	icon_size = DIR.icon_size,
	crafting_categories = {"forestry"},
	energy_source = {
		effectivity = 1,
		emissions_per_minute = 0,
		fuel_category = "chemical",
		fuel_inventory_size = 1,
		type = "burner",
		light_flicker = {
			minimum_intensity = 0,
			maximum_intensity = 0,
		},
	},
	energy_usage = "140kW",
	allowed_effects = {
		"pollution"
	},
	max_health = 450,
	resistances = {
	  {
		type = "fire",
		percent = 50
	  },
	},
	corpse = "big-remnants",
	flags = {"placeable-player", "placeable-neutral", "player-creation"},
	collision_box = { {-2.2,-2.2}, {2.2,2.2} },
	selection_box = { {-2.5,-2.5}, {2.5,2.5} },
	animation = {
		layers = {
			get_layer("machines/bronze-forestry-base", 1, 1, false, nil, 1, 320, 320, 0, 0, 320, 320, {0,0}),
			get_layer("machines/forestry-shadow", 1, 1, true, nil, 1, 448, 320, 0, 0, 448, 320, {1.0,0}),
		}	
	},
	working_visualisations = {
		{
			animation = get_layer("machines/forestry-working", 30, 6, false, nil, 0.5, 320, 320, 0, 0, 320, 320, {0,0}, "additive", nil, nil, nil, nil, "forward-then-backward"),
			light = { color = {0.5,1.0,0.5}, intensity = 0.5, size = 12, shift = {0,-0.25}},
		}	
	},
	working_sound = {
		sound = {
			filename = DIR.sound_path .. "/birds.ogg",
			volume = 1.0
		}
	},
	open_sound = {
		filename = "__base__/sound/machine-open.ogg",
		volume = 0.75
	},
	close_sound = {
		filename = "__base__/sound/machine-close.ogg",
		volume = 0.75
	},
	mined_sound = {
		filename = "__core__/sound/deconstruct-large.ogg"
	},
	vehicle_impact_sound = {
		filename = "__base__/sound/car-metal-impact.ogg",
		volume = 0.65
	},
	tile_width = 3,
	tile_height = 3,
	return_ingredients_on_change = true,
}

-- bronze forestry

data:extend({forestry})

-- iron forestry

forestry = table.deepcopy(forestry)
forestry.name = "iron-forestry"
forestry.icon = get_icon_path("iron-forestry", DIR.icon_size)
forestry.placeable_by = {item = "iron-forestry", count = 1}
forestry.minable.result = "iron-forestry"
forestry.energy_source = {
	type = "electric",
	usage_priority = "secondary-input",
	drain = "0W",
	emissions_per_minute = -2.5,
}
forestry.energy_usage = "280kW"
forestry.crafting_speed = 2
forestry.max_health = 550
forestry.animation = {
	layers = {
		get_layer("machines/iron-forestry-base", 1, 1, false, nil, 1, 320, 320, 0, 0, 320, 320, {0,0}),
		get_layer("machines/forestry-shadow", 1, 1, true, nil, 1, 448, 320, 0, 0, 448, 320, {1.0,0}),
	}	
}
forestry.working_visualisations = {
	{
		animation = get_layer("machines/forestry-working", 30, 6, false, nil, 0.25, 320, 320, 0, 0, 320, 320, {0,0}, "additive", nil, nil, nil, nil, "forward-then-backward"),
		light = { color = {0.5,1.0,0.5}, intensity = 0.5, size = 12, shift = {0,-0.25}},
	}	
}

data:extend({forestry})

------------------------------------------------------------------------------------------------------------------------------------------------------
