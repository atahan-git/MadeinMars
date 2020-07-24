------------------------------------------------------------------------------------------------------------------------------------------------------

-- electric locomotive

local loco = table.deepcopy(data.raw.locomotive["locomotive"])

loco.name = "electric-locomotive"
loco.minable.result = "electric-locomotive"
loco.burner.fuel_category = "battery"
loco.burner.smoke = nil
loco.burner.fuel_inventory_size = 1
loco.burner.burnt_inventory_size = 1
loco.max_power = "500kW"
loco.max_health = 1200
loco.icon = get_icon_path("electric-locomotive",DIR.icon_size)
loco.icon_size = DIR.icon_size
loco.minimap_representation.filename = DIR.sprites_path .. "/vehicles/electric-locomotive-minimap-representation.png"
loco.selected_minimap_representation.filename = DIR.sprites_path .. "/vehicles/electric-locomotive-selected-minimap-representation.png"

data:extend({loco})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- monowheel

local layers = {
    get_multi_layer("vehicles/monowheel-shadow", 8, 1, 1, true, 3, 8, 256, 160, 0, 0, 256, 160, {1,0.25}, nil, nil, nil, 64, false),
    get_multi_layer("vehicles/monowheel", 8, 3, 3, false, nil, 8, 128, 160, 0, 0, 128, 160, {0,-0.25}, nil, nil, nil, 64, false),
}

local monowheel = {
    alert_icon_shift = {
        0,
        -0.40625
    },
    animation = {
        layers = layers,
    },
    braking_power = "100kW",
    burner = {
        effectivity = 1,
        fuel_category = "chemical",
        fuel_inventory_size = 1,
        smoke = {
            {
                deviation = {
                    0.25,
                    0.25
                },
                frequency = 200,
                name = "car-smoke",
                position = {
                    -0.5,
                    0.8
                },
                height = 0.8,
                starting_frame = 0,
                starting_frame_deviation = 60
            },
            {
                deviation = {
                    0.25,
                    0.25
                },
                frequency = 200,
                name = "car-smoke",
                position = {
                    0.5,
                    0.8
                },
                height = 0.8,
                starting_frame = 0,
                starting_frame_deviation = 60
            }
        }
    },
    close_sound = {
        filename = "__base__/sound/car-door-close.ogg",
        volume = 0.7
    },
    collision_box = {
        {
            -0.7,
            -1
        },
        {
            0.7,
            1
        }
    },
    consumption = "125kW",
    corpse = "medium-remnants",
    crash_trigger = {
        sound = {
            {
                filename = "__base__/sound/car-crash.ogg",
                volume = 0.4
            }
        },
        type = "play-sound"
    },
    dying_explosion = "medium-explosion",
    effectivity = 0.6,
    energy_per_hit_point = 1,
    flags = {
        "placeable-neutral",
        "player-creation",
        "placeable-off-grid",
        "not-flammable"
    },
    friction = 0.002,
    icon = get_icon_path("monowheel"),
    icon_size = DIR.icon_size,
    inventory_size = 20,
    light = {
        {
            color = {
                b = 0.3,
                g = 0.77000000000000002,
                r = 0.92000000000000011
            },
            intensity = 0.6,
            minimum_darkness = 0.3,
            picture = {
                filename = "__core__/graphics/light-cone.png",
                flags = {
                    "light"
                },
                height = 200,
                priority = "extra-high",
                scale = 2,
                width = 200
            },
            shift = {
                -0.6,
                -14
            },
            size = 2,
            type = "oriented"
        },
        {
            color = {
                b = 0.3,
                g = 0.77000000000000002,
                r = 0.92000000000000011
            },
            intensity = 0.6,
            minimum_darkness = 0.3,
            picture = {
                filename = "__core__/graphics/light-cone.png",
                flags = {
                    "light"
                },
                height = 200,
                priority = "extra-high",
                scale = 2,
                width = 200
            },
            shift = {
                0.6,
                -14
            },
            size = 2,
            type = "oriented"
        }
    },
    max_health = 300,
    minable = {
        mining_time = 0.5,
        result = "monowheel"
    },
    mined_sound = {
        filename = "__core__/sound/deconstruct-medium.ogg"
    },
    name = "monowheel",
    open_sound = {
        filename = "__base__/sound/car-door-open.ogg",
        volume = 0.7
    },
    render_layer = "object",
    resistances = {
        {
            percent = 50,
            type = "fire"
        },
        {
            decrease = 50,
            percent = 30,
            type = "impact"
        },
        {
            percent = 20,
            type = "acid"
        }
    },
    rotation_speed = 0.0175,
    selection_box = {
        {
            -0.7,
            -1
        },
        {
            0.7,
            1
        }
    },
    sound_minimum_speed = 0.2,
    sound_no_fuel = {
        {
            filename = "__base__/sound/fight/car-no-fuel-1.ogg",
            volume = 0.6
        }
    },
    stop_trigger = {
        {
            sound = {
                {
                    filename = "__base__/sound/car-breaks.ogg",
                    volume = 0.6
                }
            },
            type = "play-sound"
        }
    },
    stop_trigger_speed = 0.2,
    type = "car",
    vehicle_impact_sound = {
        filename = "__base__/sound/car-metal-impact.ogg",
        volume = 0.65
    },
    weight = 900,
    working_sound = {
        activate_sound = {
            filename = "__base__/sound/car-engine-start.ogg",
            volume = 0.6
        },
        deactivate_sound = {
            filename = "__base__/sound/car-engine-stop.ogg",
            volume = 0.6
        },
        match_speed_to_activity = true,
        sound = {
            filename = "__base__/sound/car-engine.ogg",
            volume = 0.6
        }
    }
}

data:extend({monowheel})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- racing monowheel

local racing = table.deepcopy(monowheel)

racing.name = "racing-monowheel"
racing.minable.result = "racing-monowheel"
racing.icon = get_icon_path("racing-monowheel")
racing.icon_size = DIR.icon_size
racing.max_health = 400
racing.burner.smoke = nil
racing.burner.fuel_category = "battery"
racing.burner.fuel_inventory_size = 1
racing.burner.burnt_inventory_size = 1
racing.burner.effectivity = 2
racing.friction = 0.0009
racing.braking_power = "200kW"

racing.animation = {
	layers = {
		get_multi_layer("vehicles/racing-monowheel-shadow", 8, 1, 1, true, 3, 8, 256, 192, 0, 0, 256, 192, {1.5,0.25}, nil, nil, nil, 64, false),
		get_multi_layer("vehicles/racing-monowheel-base", 8, 1, 1, false, 3, 8, 224, 192, 0, 0, 224, 192, {0,-0.75}, nil, nil, nil, 64, false),
		get_multi_layer("vehicles/racing-monowheel-tracks", 8, 3, 3, false, nil, 8, 224, 192, 0, 0, 224, 192, {0,-0.75}, nil, nil, nil, 64, false),
	},
}

data:extend({racing})

------------------------------------------------------------------------------------------------------------------------------------------------------
