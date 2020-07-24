------------------------------------------------------------------------------------------------------------------------------------------------------

-- FLUID MACHINES

------------------------------------------------------------------------------------------------------------------------------------------------------

-- flare stack

local speed = 1
local animspeed = DIR.anim_speed / speed
local energy = get_scaled_energy_usage(1/5, 0)
local stack = {
    name = "flare-stack",
    type = "furnace",
    result_inventory_size = 1,
    source_inventory_size = 0,
    show_recipe_icon = false,
    icon = get_icon_path("flare-stack", DIR.icon_size),
    icon_size = DIR.icon_size,
    crafting_categories = {"voiding"},
    placeable_by = {item = "flare-stack", count = 1},
    minable = {
        mining_time = 0.5,
        result = "flare-stack"
    },
    fast_replaceable_group = "flare-stack",
    crafting_speed = speed,
    energy_usage = energy.active,
    energy_source = {
        type = "electric",
        usage_priority = "secondary-input",
        drain = energy.passive,
        emissions_per_minute = 45,
    },
    max_health = 200,
    dying_explosion = "explosion",
    resistances = {
        {
            type = "fire",
            percent = 90
        },
    },
    corpse = "small-remnants",
    flags = {"placeable-player", "placeable-neutral", "player-creation"},
    selection_box = { {-0.5,-0.5}, {0.5,0.5} },
    collision_box = { {-0.49,-0.49}, {0.49,0.49} },
    tile_width = 1,
    tile_height = 1,
    working_sound = {
        sound = {
            filename = "__base__/sound/furnace.ogg",
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
        filename = "__core__/sound/deconstruct-medium.ogg"
    },
    vehicle_impact_sound = {
        filename = "__base__/sound/car-metal-impact.ogg",
        volume = 0.65
    },
    fluid_boxes = {
        {
			base_area = 10,
            base_level = -1,
            production_type = "input",
            pipe_picture = {
				north = get_layer("machines/flare-stack-pipe-pictures", nil, nil, false, nil, nil, 46, 6, 41, 18, 128, 128, {0,1}),
				east = get_layer("machines/flare-stack-pipe-pictures", nil, nil, false, nil, nil, 2, 50, 94, 36, 128, 128, {-1,0}),
				south = get_layer("machines/flare-stack-pipe-pictures", nil, nil, false, nil, nil, 50, 26, 39, 70, 128, 128, {0,-1}), 
				west = get_layer("machines/flare-stack-pipe-pictures", nil, nil, false, nil, nil, 2, 50, 32, 36, 128, 128, {1,0}),
			},
            pipe_covers = pipecoverspictures(),
            pipe_connections = {{ type="input", position = {0, -1} }},
            secondary_draw_orders = { north = -1 }
        },
        off_when_no_fluid_recipe = false
    },
    animation = {
        layers = {
            get_layer("machines/flare-stack-base", nil, nil, false, nil, animspeed, 128, 320, 0, 0, 128, 320, {0,-1.5}),
			get_layer("machines/flare-stack-shadow", nil, nil, true, nil, animspeed, 384, 128, 0, 0, 384, 128, {2.5,0}),
        }
    },
    working_visualisations = {
        {
            animation = {
                animation_speed = animspeed,
                filename = "__base__/graphics/entity/oil-refinery/hr-oil-refinery-fire.png",
                frame_count = 60,
                height = 81,
                line_length = 10,
                scale = 0.5,
                shift = {0,-3.6},
                width = 40
            },
            light = {
				color = DIR.oil_flame_colour,
				intensity = 0.75,
				size = 6,
				shift = {0,-3.2}
			},
        },
    },
}

data:extend({stack})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- small tank

local sheets = {
	get_layer("machines/small-tank-base", nil, nil, false, nil, 1, 128, 192, 0, 0, 128, 192, {0,0}),
	get_layer("machines/small-tank-shadow", nil, nil, true, nil, 1, 256, 160, 0, 0, 256, 160, {1,0}),				
}
sheets[1].frames = 1
sheets[2].frames = 1

local pipe_covers = pipecoverspictures()
pipe_covers.north = get_layer("machines/blank", nil, nil, false, nil, nil, 64, 64, 0, 0, 64, 64, {0,0})

local circuits = circuit_connector_definitions.create (
    universal_connector_template,
    {
		{ variation = 20, main_offset = util.by_pixel_hr(14, -64), shadow_offset = util.by_pixel_hr(24, -54), show_shadow = true },
		{ variation = 20, main_offset = util.by_pixel_hr(14, -64), shadow_offset = util.by_pixel_hr(24, -54), show_shadow = true },
		{ variation = 20, main_offset = util.by_pixel_hr(14, -64), shadow_offset = util.by_pixel_hr(24, -54), show_shadow = true },
		{ variation = 20, main_offset = util.by_pixel_hr(14, -64), shadow_offset = util.by_pixel_hr(24, -54), show_shadow = true },
	}
)

data:extend({
	{
		name = "small-tank",
		type = "storage-tank",
		localised_description = {"entity-description.storage-tank-pollution"},
		minable = {
			mining_time = 0.5,
			result = "small-tank"
		},
		circuit_connector_sprites = circuits.sprites,
		circuit_wire_connection_points = circuits.points,
		circuit_wire_max_distance = 9,
		collision_box = {{-0.45,-0.45},{0.45,0.45}},
		selection_box = {{-0.5,-0.5},{0.5,0.5}},
		window_bounding_box = {{-0.25,-0.65},{0.25,0}},
		corpse = "small-remnants",
		fast_replaceable_group = "pipe",
		flags = {
			"placeable-player",
			"player-creation"
		},
		flow_length_in_ticks = 360,
		fluid_box = {
			base_area = 25,
			pipe_connections = {
				{ position = {0,-1} },
				{ position = {1,0} },
				{ position = {0,1} },
				{ position = {-1,0} },
			},
			pipe_covers = pipe_covers,
            secondary_draw_orders = { north = -1 }
		},
		icon = get_icon_path("small-tank"),
		icon_size = DIR.icon_size,
		max_health = 150,
		pictures = {
			picture = {
				sheets = sheets,
			},
			window_background = data.raw["storage-tank"]["storage-tank"].pictures.window_background,
			flow_sprite = data.raw["storage-tank"]["storage-tank"].pictures.flow_sprite,
			gas_flow = data.raw["storage-tank"]["storage-tank"].pictures.gas_flow,
			fluid_background = get_layer("machines/fluid-background-narrow", nil, nil, false, nil, nil, 16, 15, 0, 0, 16, 15, {0,0}, nil, nil, nil, nil, nil, nil, 1),
		},
		two_direction_only = true,
		vehicle_impact_sound = {
			filename = "__base__/sound/car-metal-impact.ogg",
			volume = 0.65
		},
		working_sound = {
			apparent_volume = 1.5,
			match_volume_to_activity = true,
			max_sounds_per_type = 3,
			sound = {
				filename = "__base__/sound/storage-tank.ogg",
				volume = 0.8
			}
		}
	}
})

------------------------------------------------------------------------------------------------------------------------------------------------------
