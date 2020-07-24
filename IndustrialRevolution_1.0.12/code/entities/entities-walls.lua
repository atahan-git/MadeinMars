------------------------------------------------------------------------------------------------------------------------------------------------------

-- WALLS

------------------------------------------------------------------------------------------------------------------------------------------------------

-- stone wall resistance buff (explosions)

data.raw.wall["stone-wall"].resistances = {
    {
        decrease = 3,
        percent = 20,
        type = "physical"
    },
    {
        decrease = 40,
        percent = 60,
        type = "impact"
    },
    {
        decrease = 10,
        percent = 75,
        type = "explosion"
    },
    {
        percent = 100,
        type = "fire"
    },
    {
        percent = 75,
        type = "acid"
    },
    {
        percent = 75,
        type = "laser"
    },
    {
        percent = 75,
        type = "electric"
    }
}

------------------------------------------------------------------------------------------------------------------------------------------------------

local steel = table.deepcopy(data.raw.wall["stone-wall"])

steel.name = "steel-plate-wall"
steel.corpse = "small-remnants"
steel.icon = get_icon_path("steel-plate-wall")
steel.icon_size = DIR.icon_size
steel.max_health = 1000
steel.minable.result = "steel-plate-wall"
steel.mined_sound.filename = "__core__/sound/deconstruct-large.ogg"
steel.vehicle_impact_sound.filename = "__base__/sound/car-metal-impact.ogg"

steel.pictures = {
	corner_left_down = {
		layers = {
			get_layer("walls/steel-plate-wall-shadow", 1, 1, true, 8, nil, 160, 160, 0, 5 * 160, 160, 160),
			get_layer("walls/steel-plate-wall-base", -8, 8, false, nil, nil, 64, 128, 0, 5 * 128, 64, 128)
		}
	},
	corner_right_down = {
		layers = {
			get_layer("walls/steel-plate-wall-shadow", 1, 1, true, 8, nil, 160, 160, 0, 7 * 160, 160, 160),
			get_layer("walls/steel-plate-wall-base", -8, 8, false, nil, nil, 64, 128, 0, 7 * 128, 64, 128)
		}
	},
	ending_left = {
		layers = {
			get_layer("walls/steel-plate-wall-shadow", 1, 1, true, 8, nil, 160, 160, 0, 1 * 160, 160, 160),
			get_layer("walls/steel-plate-wall-base", -8, 8, false, nil, nil, 64, 128, 0, 1 * 128, 64, 128)
		}
	},
	ending_right = {
		layers = {
			get_layer("walls/steel-plate-wall-shadow", 1, 1, true, 8, nil, 160, 160, 0, 3 * 160, 160, 160),
			get_layer("walls/steel-plate-wall-base", -8, 8, false, nil, nil, 64, 128, 0, 3 * 128, 64, 128)
		}
	},
	single = {
		layers = {
			get_layer("walls/steel-plate-wall-shadow", 1, 1, true, 8, nil, 160, 160, 0, 0 * 160, 160, 160),
			get_layer("walls/steel-plate-wall-base", -8, 8, false, nil, nil, 64, 128, 0, 0 * 128, 64, 128)
		}
	},
	straight_horizontal = {
		layers = {
			get_layer("walls/steel-plate-wall-shadow", 1, 1, true, 8, nil, 160, 160, 0, 2 * 160, 160, 160),
			get_layer("walls/steel-plate-wall-base", -8, 8, false, nil, nil, 64, 128, 0, 2 * 128, 64, 128)
		}
	},
	straight_vertical = {
		layers = {
			get_layer("walls/steel-plate-wall-shadow", 1, 1, true, 8, nil, 160, 160, 0, 4 * 160, 160, 160),
			get_layer("walls/steel-plate-wall-base", -8, 8, false, nil, nil, 64, 128, 0, 4 * 128, 64, 128)
		}
	},
	t_up = {
		layers = {
			get_layer("walls/steel-plate-wall-shadow", 1, 1, true, 8, nil, 160, 160, 0, 6 * 160, 160, 160),
			get_layer("walls/steel-plate-wall-base", -8, 8, false, nil, nil, 64, 128, 0, 6 * 128, 64, 128)
		}
	},
	filling = get_layer("walls/steel-plate-wall-filler", -8, 8, false, nil, nil, 64, 64, 0, 0, 64, 64),
	gate_connection_patch = {
		sheets = {
			get_layer("walls/steel-plate-wall-gate-shadow", nil, nil, true, nil, nil, 128, 128, 0, 0, 128, 128, {0.5,0.5}),
			get_layer("walls/steel-plate-wall-gate-patch", nil, nil, false, nil, nil, 64, 192, nil, nil, 64, 192),
		},
	},
	water_connection_patch = {
		sheets = {
			get_layer("walls/steel-plate-wall-water-shadow", nil, nil, true, nil, nil, 192, 160, 0, 0, 192, 160, {0.5,0.75}),
			get_layer("walls/steel-plate-wall-water-patch", nil, nil, false, nil, nil, 128, 160, 0, 0, 128, 160, {0,-0.25}),
		},
	},
}

for _,picture in pairs(steel.pictures) do
	if picture.layers then picture.layers[1].shift = {0.75,0.75} end
end

data:extend({steel})

data.raw.wall["stone-wall"].next_upgrade = "steel-plate-wall"

------------------------------------------------------------------------------------------------------------------------------------------------------
