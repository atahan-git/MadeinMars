------------------------------------------------------------------------------------------------------------------------------------------------------

-- DRILLS

------------------------------------------------------------------------------------------------------------------------------------------------------

-- copper drill

local speed = DIR.basic_drill_speed
local energy = get_scaled_energy_usage(2, 0)

local drillbase = get_layer("machines/copper-drill-base", 60, 10, false, nil, 0.5, 192, 256, 0, 0, 192, 256, {0,-0.5}, nil, nil, nil, nil, nil, "forward-then-backward")
local drillshadow = get_layer("machines/copper-drill-shadow", 60, 6, true, nil, 0.5, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, nil, nil, "forward-then-backward")

local drill = {
	name = "burner-mining-drill",
	type = "mining-drill",
	icon = get_icon_path("burner-mining-drill", DIR.icon_size),
	icon_size = DIR.icon_size,
	minable = {
		mining_time = 1.0,
		result = "burner-mining-drill"
	},
	allowed_effects = {
		"pollution"
	},
	module_specification = {
		module_info_icon_shift = {0, 0},
		module_slots = 1
	},
	animations = {
		north = {
			layers = {
				get_layer("machines/copper-drill-underlay-north-shadow", nil, nil, true, 120, 0.5, 256, 256, 0, 0, 256, 256, {0,0}),
				drillshadow,
				get_layer("machines/copper-drill-underlay-north", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {0,0}),
				drillbase,
			}
		},
		east = {
			layers = {
				get_layer("machines/copper-drill-underlay-east-shadow", nil, nil, true, 120, 0.5, 256, 256, 0, 0, 256, 256, {0,0}),
				drillshadow,
				get_layer("machines/copper-drill-underlay-east", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {0,0}),
				drillbase,
			}
		},
		south = {
			layers = {
				get_layer("machines/copper-drill-underlay-south-shadow", nil, nil, true, 120, 0.5, 256, 256, 0, 0, 256, 256, {0,0}),
				drillshadow,
				get_layer("machines/copper-drill-underlay-south", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {0,0}),
				drillbase,
			}
		},
		west = {
			layers = {
				get_layer("machines/copper-drill-underlay-west-shadow", nil, nil, true, 120, 0.5, 256, 256, 0, 0, 256, 256, {0,0}),
				drillshadow,
				get_layer("machines/copper-drill-underlay-west", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {0,0}),
				drillbase,
			}
		},
	},
	base_render_layer = "object",
	circuit_connector_sprites = nil,
	circuit_wire_connection_points = nil,
	circuit_wire_max_distance = nil, --9
	collision_box = { {-1.4,-1.4}, {1.4,1.4} },
	selection_box = { {-1.5,-1.5}, {1.5,1.5} },
	drawing_box = { {-2,-2}, {2,2} },
	corpse = "big-remnants",
	energy_source = {
		effectivity = 1,
		emissions_per_minute = 30,
		fuel_category = "chemical",
		fuel_inventory_size = 1,
		smoke = {
		  {
			deviation = {0.1, 0.1},
			frequency = 1,
			name = "smoke",
			north_position = {0, -1.75},
			south_position = {0, -1.75},
			east_position = {0, -1.75},
			west_position = {0, -1.75},
		  }
		},
		type = "burner"
	},
	energy_usage = energy.active,
	fast_replaceable_group = "mining-drill",
	flags = {"placeable-player", "placeable-neutral", "player-creation"},
	max_health = 150,
	mining_speed = speed,
	monitor_visualization_tint = {
		r = 78,
		g = 173,
		b = 255,
	},
	radius_visualisation_picture = {
		filename = "__base__/graphics/entity/electric-mining-drill/electric-mining-drill-radius-visualization.png",
		height = 10,
		width = 10
	},
	resource_categories = {"basic-solid"},
	resource_searching_radius = 2.49,
	vehicle_impact_sound = {
		filename = "__base__/sound/car-metal-impact.ogg",
		volume = 0.65
	},
	working_sound = {
		sound = {
		  filename = "__base__/sound/burner-mining-drill.ogg",
		  volume = 0.8
		}
	},
	vector_to_place_result = {0, -1.85},
}

data:extend({drill})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- iron drill

local speed = DIR.basic_drill_speed * 2
local energy = get_scaled_energy_usage(2, 0)
local circuits = circuit_connector_definitions.create (
    universal_connector_template,
    {
		{ variation = 4, main_offset = util.by_pixel_hr(-10, -162), shadow_offset = util.by_pixel_hr(0, -152), show_shadow = true },
		{ variation = 2, main_offset = util.by_pixel_hr(154, -5), shadow_offset = util.by_pixel_hr(164, 5), show_shadow = true },
		{ variation = 0, main_offset = util.by_pixel_hr(10, 142), shadow_offset = util.by_pixel_hr(20, 152), show_shadow = true },
		{ variation = 6, main_offset = util.by_pixel_hr(-154, 5), shadow_offset = util.by_pixel_hr(-144, 15), show_shadow = true },
	}
)

local drill = {
	name = "electric-mining-drill",
	type = "mining-drill",
	icon = get_icon_path("electric-mining-drill", DIR.icon_size),
	icon_size = DIR.icon_size,
	minable = {
		mining_time = 1.0,
		result = "electric-mining-drill"
	},
	allowed_effects = {
        "consumption",
        "speed",
        "pollution"
	},
	module_specification = {
		module_info_icon_shift = {0, 0},
		module_slots = 2
	},
	animations = {
		layers = {
			get_layer("machines/iron-drill-shadow", 60, 6, true, nil, 0.5, 384, 224, 0, 0, 384, 224, {1,0.25}, nil, nil, nil, nil, nil, "forward-then-backward"),			
			get_layer("machines/iron-drill-base", 60, 10, false, nil, 0.5, 256, 256, 0, 0, 256, 256, {0,-0.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
		}
	},
	base_picture = {
		north = {
			layers = {
				get_layer("machines/iron-drill-underlay-north", nil, nil, false, nil, nil, 352, 352, 0, 0, 352, 352, {0,0}),
				get_layer("machines/iron-drill-underlay-north-shadow", nil, nil, true, nil, nil, 400, 384, 0, 0, 400, 384, {0.125,0}),
			}
		},
		east = {
			layers = {
				get_layer("machines/iron-drill-underlay-east", nil, nil, false, nil, nil, 352, 352, 0, 0, 352, 352, {0,0}),
				get_layer("machines/iron-drill-underlay-east-shadow", nil, nil, true, nil, nil, 400, 384, 0, 0, 400, 384, {0.125,0}),
			}
		},
		south = {
			layers = {
				get_layer("machines/iron-drill-underlay-south", nil, nil, false, nil, nil, 352, 352, 0, 0, 352, 352, {0,0}),
				get_layer("machines/iron-drill-underlay-south-shadow", nil, nil, true, nil, nil, 400, 384, 0, 0, 400, 384, {0.125,0}),
			}
		},
		west = {
			layers = {
				get_layer("machines/iron-drill-underlay-west", nil, nil, false, nil, nil, 352, 352, 0, 0, 352, 352, {0,0}),
				get_layer("machines/iron-drill-underlay-west-shadow", nil, nil, true, nil, nil, 400, 384, 0, 0, 400, 384, {0.125,0}),
			}
		},
	},
	input_fluid_patch_shadow_animations = {
		north = {
			layers = {
				get_layer("machines/iron-drill-connector-south", 60, 10, false, nil, 0.5, 80, 224, 0, 0, 80, 224, {0,1.25}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-connector-east", 60, 10, false, nil, 0.5, 160, 128, 0, 0, 160, 128, {1.75,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-connector-west", 60, 10, false, nil, 0.5, 160, 128, 0, 0, 160, 128, {-1.75,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-pipe-picture-south", nil, nil, false, 120, 0.5, 64, 24, 0, 0, 64, 24, {2/64,2+(21/64)}),
				get_layer("machines/iron-drill-pipes-se", nil, nil, false, 120, 0.5, 192, 192, 0, 0, 192, 192, {1.5,1.5}),
				get_layer("machines/iron-drill-pipes-sw", nil, nil, false, 120, 0.5, 192, 192, 0, 0, 192, 192, {-1.5,1.5}),
				get_layer("machines/iron-drill-pipes-shadow-se", 60, 10, false, nil, 0.5, 64, 128, 0, 0, 64, 128, {2,1}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-e", 60, 6, true, nil, 0.5, 320, 64, 0, 0, 320, 64, {0,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-w", 60, 6, true, nil, 0.5, 320, 64, 0, 0, 320, 64, {0,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-s", 60, 10, true, nil, 0.5, 128, 192, 0, 0, 128, 192, {1,1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
			}
		},
		east = {
			layers = {
				get_layer("machines/iron-drill-connector-north", 60, 10, false, nil, 0.5, 80, 128, 0, 0, 80, 128, {0,-2}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-connector-south", 60, 10, false, nil, 0.5, 80, 224, 0, 0, 80, 224, {0,1.25}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-connector-west", 60, 10, false, nil, 0.5, 160, 128, 0, 0, 160, 128, {-1.75,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-pipe-picture-south", nil, nil, false, 120, 0.5, 64, 24, 0, 0, 64, 24, {2/64,2+(21/64)}),
				get_layer("machines/iron-drill-pipes-nw", nil, nil, false, 120, 0.5, 192, 192, 0, 0, 192, 192, {-1.5,-1.5}),
				get_layer("machines/iron-drill-pipes-sw", nil, nil, false, 120, 0.5, 192, 192, 0, 0, 192, 192, {-1.5,1.5}),
				get_layer("machines/iron-drill-tube-shadow-w", 60, 6, true, nil, 0.5, 320, 64, 0, 0, 320, 64, {0,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-s", 60, 10, true, nil, 0.5, 128, 192, 0, 0, 128, 192, {1,1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-n", 60, 10, true, nil, 0.5, 128, 192, 0, 0, 128, 192, {1,-1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
			}
		},
		south = {
			layers = {
				get_layer("machines/iron-drill-connector-north", 60, 10, false, nil, 0.5, 80, 128, 0, 0, 80, 128, {0,-2}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-connector-east", 60, 10, false, nil, 0.5, 160, 128, 0, 0, 160, 128, {1.75,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-connector-west", 60, 10, false, nil, 0.5, 160, 128, 0, 0, 160, 128, {-1.75,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-pipes-ne", nil, nil, false, 120, 0.5, 192, 192, 0, 0, 192, 192, {1.5,-1.5}),
				get_layer("machines/iron-drill-pipes-nw", nil, nil, false, 120, 0.5, 192, 192, 0, 0, 192, 192, {-1.5,-1.5}),
				get_layer("machines/iron-drill-pipes-shadow-ne", 60, 10, false, nil, 0.5, 64, 128, 0, 0, 64, 128, {2,-1}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-e", 60, 6, true, nil, 0.5, 320, 64, 0, 0, 320, 64, {0,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-w", 60, 6, true, nil, 0.5, 320, 64, 0, 0, 320, 64, {0,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-n", 60, 10, true, nil, 0.5, 128, 192, 0, 0, 128, 192, {1,-1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
			}
		},
		west = {
			layers = {
				get_layer("machines/iron-drill-connector-north", 60, 10, false, nil, 0.5, 80, 128, 0, 0, 80, 128, {0,-2}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-connector-south", 60, 10, false, nil, 0.5, 80, 224, 0, 0, 80, 224, {0,1.25}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-connector-east", 60, 10, false, nil, 0.5, 160, 128, 0, 0, 160, 128, {1.75,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-pipe-picture-south", nil, nil, false, 120, 0.5, 64, 24, 0, 0, 64, 24, {2/64,2+(21/64)}),
				get_layer("machines/iron-drill-pipes-ne", nil, nil, false, 120, 0.5, 192, 192, 0, 0, 192, 192, {1.5,-1.5}),
				get_layer("machines/iron-drill-pipes-se", nil, nil, false, 120, 0.5, 192, 192, 0, 0, 192, 192, {1.5,1.5}),
				get_layer("machines/iron-drill-pipes-shadow-ne", 60, 10, false, nil, 0.5, 64, 128, 0, 0, 64, 128, {2,-1}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-pipes-shadow-se", 60, 10, false, nil, 0.5, 64, 128, 0, 0, 64, 128, {2,1}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-e", 60, 6, true, nil, 0.5, 320, 64, 0, 0, 320, 64, {0,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-s", 60, 10, true, nil, 0.5, 128, 192, 0, 0, 128, 192, {1,1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-tube-shadow-n", 60, 10, true, nil, 0.5, 128, 192, 0, 0, 128, 192, {1,-1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
			}
		},
	},
	input_fluid_patch_shadow_sprites = {
		north = {
			layers = {
				get_layer("machines/iron-drill-patch-north-shadow", nil, nil, true, nil, 120, 448, 384, 0, 0, 448, 384, {0.5,0}),
			},
		},
		east = {
			layers = {
				get_layer("machines/iron-drill-patch-east-shadow", nil, nil, true, nil, 120, 448, 384, 0, 0, 448, 384, {0.5,0}),
			},
		},
		south = {
			layers = {
				get_layer("machines/iron-drill-patch-south-shadow", nil, nil, true, nil, 120, 448, 384, 0, 0, 448, 384, {0.5,0}),
			},
		},
		west = {
			layers = {
				get_layer("machines/iron-drill-patch-west-shadow", nil, nil, true, nil, 120, 448, 384, 0, 0, 448, 384, {0.5,0}),
			},
		},
	},
	input_fluid_patch_window_base_sprites = {{
		north = {
			layers = {
				get_layer("machines/iron-drill-window-base-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {2.0,0}, "additive"),
				get_layer("machines/iron-drill-window-base-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,2.0}, "additive"),
				get_layer("machines/iron-drill-window-base-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-2.0,0}, "additive"),
			},
		},
		east = {
			layers = {
				get_layer("machines/iron-drill-window-base-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-2.0}, "additive"),
				get_layer("machines/iron-drill-window-base-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,2.0}, "additive"),
				get_layer("machines/iron-drill-window-base-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-2.0,0}, "additive"),
			},
		},
		south = {
			layers = {
				get_layer("machines/iron-drill-window-base-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-2.0}, "additive"),
				get_layer("machines/iron-drill-window-base-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {2.0,0}, "additive"),
				get_layer("machines/iron-drill-window-base-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-2.0,0}, "additive"),
			},
		},
		west = {
			layers = {
				get_layer("machines/iron-drill-window-base-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-2.0}, "additive"),
				get_layer("machines/iron-drill-window-base-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {2.0,0}, "additive"),
				get_layer("machines/iron-drill-window-base-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,2.0}, "additive"),
			},
		},
	}},
	input_fluid_patch_window_flow_sprites = {{
		north = {
			layers = {
				get_layer("machines/iron-drill-window-flow-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {2.0,0}, "additive"),
				get_layer("machines/iron-drill-window-flow-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,2.0}, "additive"),
				get_layer("machines/iron-drill-window-flow-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-2.0,0}, "additive"),
			},
		},
		east = {
			layers = {
				get_layer("machines/iron-drill-window-flow-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-2.0}, "additive"),
				get_layer("machines/iron-drill-window-flow-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,2.0}, "additive"),
				get_layer("machines/iron-drill-window-flow-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-2.0,0}, "additive"),
			},
		},
		south = {
			layers = {
				get_layer("machines/iron-drill-window-flow-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-2.0}, "additive"),
				get_layer("machines/iron-drill-window-flow-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {2.0,0}, "additive"),
				get_layer("machines/iron-drill-window-flow-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-2.0,0}, "additive"),
			},
		},
		west = {
			layers = {
				get_layer("machines/iron-drill-window-flow-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-2.0}, "additive"),
				get_layer("machines/iron-drill-window-flow-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {2.0,0}, "additive"),
				get_layer("machines/iron-drill-window-flow-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,2.0}, "additive"),
			},
		},
	}},
	input_fluid_box = {
        base_area = 1,
        base_level = -1,
        height = 2,
        pipe_connections = {
			{ position = { -3, 0 } },
			{ position = { 3, 0 } },
			{ position = { 0, 3 } },
        },
        pipe_covers = pipecoverspictures(),
		production_type = "input-output",
	},
	base_render_layer = "object",
	circuit_connector_sprites = circuits.sprites,
	circuit_wire_connection_points = circuits.points,
	circuit_wire_max_distance = default_circuit_wire_max_distance,
	collision_box = { {-2.4,-2.4}, {2.4,2.4} },
	corpse = "big-remnants",
	energy_source = {
		emissions_per_minute = 25,
		usage_priority = "secondary-input",
		drain = energy.passive,
		type = "electric"
	},
	energy_usage = energy.active,
	fast_replaceable_group = "mining-drill",
	flags = {"placeable-player", "placeable-neutral", "player-creation"},
	max_health = 400,
	mining_speed = speed,
	monitor_visualization_tint = {
		r = 78,
		g = 173,
		b = 255,
	},
	radius_visualisation_picture = {
		filename = "__base__/graphics/entity/electric-mining-drill/electric-mining-drill-radius-visualization.png",
		height = 10,
		width = 10
	},
	resource_categories = {"basic-solid"},
	resource_searching_radius = 4.49,
	selection_box = { {-2.5,-2.5}, {2.5,2.5} },
	vehicle_impact_sound = {
		filename = "__base__/sound/car-metal-impact.ogg",
		volume = 0.65
	},
	working_sound = {
		sound = {
		  filename = "__base__/sound/electric-mining-drill.ogg",
		  volume = 0.8
		},
        idle_sound = {
          filename = "__base__/sound/idle1.ogg",
          volume = 0.6
        },
	},
	vector_to_place_result = {0, -2.85},
}

data:extend({drill})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- chrome drill

local speed = DIR.basic_drill_speed * 4
local energy = get_scaled_energy_usage(5, 0)
local circuits = circuit_connector_definitions.create (
    universal_connector_template,
    {
		{ variation = 4, main_offset = util.by_pixel_hr(-10, -226), shadow_offset = util.by_pixel_hr(0, -216), show_shadow = true },
		{ variation = 2, main_offset = util.by_pixel_hr(218, -5), shadow_offset = util.by_pixel_hr(228, 5), show_shadow = true },
		{ variation = 0, main_offset = util.by_pixel_hr(10, 206), shadow_offset = util.by_pixel_hr(20, 196), show_shadow = true },
		{ variation = 6, main_offset = util.by_pixel_hr(-218, 5), shadow_offset = util.by_pixel_hr(-208, 15), show_shadow = true },
	}
)

local drill = {
	name = "chrome-drill",
	type = "mining-drill",
	icon = get_icon_path("chrome-drill", DIR.icon_size),
	icon_size = DIR.icon_size,
	minable = {
		mining_time = 1.0,
		result = "chrome-drill"
	},
	allowed_effects = {
        "consumption",
        "speed",
        "pollution"
	},
	module_specification = {
		module_info_icon_shift = {0, 0},
		module_slots = 4
	},
	animations = {
		layers = {
			get_layer("machines/chrome-drill-shadow", 60, 6, true, nil, 0.5, 544, 288, 0, 0, 544, 288, {1.25,0.25}, nil, nil, nil, nil, nil, "forward-then-backward"),			
			get_layer("machines/chrome-drill-base", 60, 10, false, nil, 0.5, 320, 384, 0, 0, 320, 384, {0,-0.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
		}
	},
	base_picture = {
		north = {
			layers = {
				get_layer("machines/chrome-drill-edge-rail", nil, nil, false, nil, nil, 512, 512, 0, 0, 512, 512, {0,0}),
				get_layer("machines/chrome-drill-underlay-north", nil, nil, false, nil, nil, 192, 128, 0, 0, 192, 128, {0,-(3+3/64)}),
				get_layer("machines/chrome-drill-underlay-north-shadow", nil, nil, true, nil, nil, 544, 512, 0, 0, 544, 512, {0.25,0}),
			}
		},
		east = {
			layers = {
				get_layer("machines/chrome-drill-edge-rail", nil, nil, false, nil, nil, 512, 512, 0, 0, 512, 512, {0,0}),
				get_layer("machines/chrome-drill-underlay-east", nil, nil, false, nil, nil, 128, 192, 0, 0, 128, 192, {3,0}),
				get_layer("machines/chrome-drill-underlay-east-shadow", nil, nil, true, nil, nil, 544, 512, 0, 0, 544, 512, {0.25,0}),
			}
		},
		south = {
			layers = {
				get_layer("machines/chrome-drill-edge-rail", nil, nil, false, nil, nil, 512, 512, 0, 0, 512, 512, {0,0}),
				get_layer("machines/chrome-drill-underlay-south", nil, nil, false, nil, nil, 192, 128, 0, 0, 192, 128, {0,3-(3/64)}),
				get_layer("machines/chrome-drill-underlay-south-shadow", nil, nil, true, nil, nil, 544, 512, 0, 0, 544, 512, {0.25,0}),
			}
		},
		west = {
			layers = {
				get_layer("machines/chrome-drill-edge-rail", nil, nil, false, nil, nil, 512, 512, 0, 0, 512, 512, {0,0}),
				get_layer("machines/chrome-drill-underlay-west", nil, nil, false, nil, nil, 128, 192, 0, 0, 128, 192, {-3,0}),
				get_layer("machines/chrome-drill-underlay-west-shadow", nil, nil, true, nil, nil, 544, 512, 0, 0, 544, 512, {0.25,0}),
			}
		},
	},
	input_fluid_patch_shadow_animations = {
		north = {
			layers = {
				get_layer("machines/chrome-drill-connector-south", 60, 10, false, nil, 0.5, 128, 192, 0, 0, 128, 192, {0,2.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-connector-east", 60, 10, false, nil, 0.5, 192, 128, 0, 0, 192, 128, {2.5,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-connector-west", 60, 10, false, nil, 0.5, 192, 128, 0, 0, 192, 128, {-2.5,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-pipe-picture-south", nil, nil, false, 120, 0.5, 64, 24, 0, 0, 64, 24, {2/64,3+(21/64)}),
				get_layer("machines/chrome-drill-pipes-se", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {2,2}),
				get_layer("machines/chrome-drill-pipes-sw", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {-2,2}),
				get_layer("machines/chrome-drill-pipes-shadow-se", 60, 10, false, nil, 0.5, 64, 192, 0, 0, 64, 192, {3,1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
			}
		},
		east = {
			layers = {
				get_layer("machines/chrome-drill-connector-north", 60, 10, false, nil, 0.5, 128, 192, 0, 0, 128, 192, {0,-2.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-connector-south", 60, 10, false, nil, 0.5, 128, 192, 0, 0, 128, 192, {0,2.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-connector-west", 60, 10, false, nil, 0.5, 192, 128, 0, 0, 192, 128, {-2.5,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-pipe-picture-south", nil, nil, false, 120, 0.5, 64, 24, 0, 0, 64, 24, {2/64,3+(21/64)}),
				get_layer("machines/chrome-drill-pipes-nw", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {-2,-2}),
				get_layer("machines/chrome-drill-pipes-sw", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {-2,2}),
			}
		},
		south = {
			layers = {
				get_layer("machines/chrome-drill-connector-north", 60, 10, false, nil, 0.5, 128, 192, 0, 0, 128, 192, {0,-2.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-connector-east", 60, 10, false, nil, 0.5, 192, 128, 0, 0, 192, 128, {2.5,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-connector-west", 60, 10, false, nil, 0.5, 192, 128, 0, 0, 192, 128, {-2.5,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-pipes-ne", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {2,-2}),
				get_layer("machines/chrome-drill-pipes-nw", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {-2,-2}),
				get_layer("machines/chrome-drill-pipes-shadow-ne", 60, 10, false, nil, 0.5, 64, 192, 0, 0, 64, 192, {3,-1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
			}
		},
		west = {
			layers = {
				get_layer("machines/chrome-drill-connector-north", 60, 10, false, nil, 0.5, 128, 192, 0, 0, 128, 192, {0,-2.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-connector-south", 60, 10, false, nil, 0.5, 128, 192, 0, 0, 128, 192, {0,2.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-connector-east", 60, 10, false, nil, 0.5, 192, 128, 0, 0, 192, 128, {2.5,0}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/iron-drill-pipe-picture-south", nil, nil, false, 120, 0.5, 64, 24, 0, 0, 64, 24, {2/64,3+(21/64)}),
				get_layer("machines/chrome-drill-pipes-ne", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {2,-2}),
				get_layer("machines/chrome-drill-pipes-se", nil, nil, false, 120, 0.5, 256, 256, 0, 0, 256, 256, {2,2}),
				get_layer("machines/chrome-drill-pipes-shadow-ne", 60, 10, false, nil, 0.5, 64, 192, 0, 0, 64, 192, {3,-1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
				get_layer("machines/chrome-drill-pipes-shadow-se", 60, 10, false, nil, 0.5, 64, 192, 0, 0, 64, 192, {3,1.5}, nil, nil, nil, nil, nil, "forward-then-backward"),
			}
		},
	},
	input_fluid_patch_shadow_sprites = {
		north = {
			layers = {
				get_layer("machines/chrome-drill-patch-north-shadow", nil, nil, true, nil, 120, 544, 512, 0, 0, 544, 512, {0.25,0}),
			},
		},
		east = {
			layers = {
				get_layer("machines/chrome-drill-patch-east-shadow", nil, nil, true, nil, 120, 544, 512, 0, 0, 544, 512, {0.25,0}),
			},
		},
		south = {
			layers = {
				get_layer("machines/chrome-drill-patch-south-shadow", nil, nil, true, nil, 120, 544, 512, 0, 0, 544, 512, {0.25,0}),
			},
		},
		west = {
			layers = {
				get_layer("machines/chrome-drill-patch-west-shadow", nil, nil, true, nil, 120, 544, 512, 0, 0, 544, 512, {0.25,0}),
			},
		},
	},
	input_fluid_patch_window_base_sprites = {{
		north = {
			layers = {
				get_layer("machines/iron-drill-window-base-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {3,0}, "additive"),
				get_layer("machines/iron-drill-window-base-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,3}, "additive"),
				get_layer("machines/iron-drill-window-base-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-3,0}, "additive"),
			},
		},
		east = {
			layers = {
				get_layer("machines/iron-drill-window-base-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-3}, "additive"),
				get_layer("machines/iron-drill-window-base-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,3}, "additive"),
				get_layer("machines/iron-drill-window-base-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-3,0}, "additive"),
			},
		},
		south = {
			layers = {
				get_layer("machines/iron-drill-window-base-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-3}, "additive"),
				get_layer("machines/iron-drill-window-base-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {3,0}, "additive"),
				get_layer("machines/iron-drill-window-base-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-3,0}, "additive"),
			},
		},
		west = {
			layers = {
				get_layer("machines/iron-drill-window-base-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-3}, "additive"),
				get_layer("machines/iron-drill-window-base-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {3,0}, "additive"),
				get_layer("machines/iron-drill-window-base-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,3}, "additive"),
			},
		},
	}},
	input_fluid_patch_window_flow_sprites = {{
		north = {
			layers = {
				get_layer("machines/iron-drill-window-flow-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {3,0}, "additive"),
				get_layer("machines/iron-drill-window-flow-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,3}, "additive"),
				get_layer("machines/iron-drill-window-flow-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-3,0}, "additive"),
			},
		},
		east = {
			layers = {
				get_layer("machines/iron-drill-window-flow-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-3}, "additive"),
				get_layer("machines/iron-drill-window-flow-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,3}, "additive"),
				get_layer("machines/iron-drill-window-flow-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-3,0}, "additive"),
			},
		},
		south = {
			layers = {
				get_layer("machines/iron-drill-window-flow-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-3}, "additive"),
				get_layer("machines/iron-drill-window-flow-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {3,0}, "additive"),
				get_layer("machines/iron-drill-window-flow-w", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {-3,0}, "additive"),
			},
		},
		west = {
			layers = {
				get_layer("machines/iron-drill-window-flow-n", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-3}, "additive"),
				get_layer("machines/iron-drill-window-flow-e", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {3,0}, "additive"),
				get_layer("machines/iron-drill-window-flow-s", nil, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,3}, "additive"),
			},
		},
	}},
	input_fluid_box = {
        base_area = 1,
        base_level = -1,
        height = 2,
        pipe_connections = {
			{ position = { -4, 0 } },
			{ position = { 4, 0 } },
			{ position = { 0, 4 } },
        },
        pipe_covers = pipecoverspictures(),
		production_type = "input-output",
	},
	base_render_layer = "object",
	circuit_connector_sprites = circuits.sprites,
	circuit_wire_connection_points = circuits.points,
	circuit_wire_max_distance = default_circuit_wire_max_distance,
	collision_box = { {-3.4,-3.4}, {3.4,3.4} },
	corpse = "big-remnants",
	energy_source = {
		emissions_per_minute = 40,
		usage_priority = "secondary-input",
		drain = energy.passive,
		type = "electric"
	},
	energy_usage = energy.active,
	fast_replaceable_group = "mining-drill",
	flags = {"placeable-player", "placeable-neutral", "player-creation"},
	max_health = 1000,
	mining_speed = speed,
	monitor_visualization_tint = {
		r = 78,
		g = 173,
		b = 255,
	},
	radius_visualisation_picture = {
		filename = "__base__/graphics/entity/electric-mining-drill/electric-mining-drill-radius-visualization.png",
		height = 10,
		width = 10
	},
	resource_categories = {"basic-solid"},
	resource_searching_radius = 6.49,
	selection_box = { {-3.5,-3.5}, {3.5,3.5} },
	vehicle_impact_sound = {
		filename = "__base__/sound/car-metal-impact.ogg",
		volume = 0.65
	},
	working_sound = {
		sound = {
		  filename = DIR.sound_path.."/heavy-drill.ogg",
		  volume = 1
		},
        idle_sound = {
          filename = "__base__/sound/idle1.ogg",
          volume = 0.6
        },
	},
	vector_to_place_result = {0, -3.85},
}

data:extend({drill})

------------------------------------------------------------------------------------------------------------------------------------------------------

