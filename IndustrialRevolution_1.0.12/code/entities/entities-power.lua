------------------------------------------------------------------------------------------------------------------------------------------------------

-- POWER MACHINES

------------------------------------------------------------------------------------------------------------------------------------------------------

-- iron-battery-charger

data:extend({{
    name = "iron-battery-charger",
    type = "furnace",
    placeable_by = {item = "iron-battery-charger", count = 1},
    minable = {
        mining_time = 0.5,
        result = "iron-battery-charger"
    },
    crafting_speed = 1,
    source_inventory_size = 1,
    result_inventory_size = 1,
    show_recipe_icon = false,
    crafting_categories = {"charging"},
    order = "z-iron-battery-charger",
    icon = get_icon_path("iron-battery-charger", DIR.icon_size),
    icon_size = DIR.icon_size,
    energy_usage = "625kW",
    energy_source = {
        type = "electric",
        usage_priority = "secondary-input",
        drain = "0kW",
    },
    allowed_effects = nil,
    module_specification = nil,
    max_health = 100,
    dying_explosion = "medium-explosion",
    resistances = {
        {
            type = "fire",
            percent = 80
        },
    },
    corpse = "medium-remnants",
    flags = {"placeable-player", "placeable-neutral", "player-creation"},
    collision_box = { {-0.9,-0.9}, {0.9,0.9} },
    selection_box = { {-1.0,-1.0}, {1.0,1.0} },
    animation = {
        layers = {
            get_layer("machines/iron-battery-charger-base", 1, 1, false, nil, DIR.anim_speed, 128, 160, 0, 0, 128, 160, {0,-0.25}),
            get_layer("machines/iron-battery-charger-shadow", 1, 1, true, nil, DIR.anim_speed, 256, 160, 0, 0, 256, 160, {1.0,-0.25}),
        }
    },
    working_visualisations = {
        {
            animation = get_layer("machines/iron-battery-charger-working", 30, 6, false, nil, DIR.anim_speed, 128, 160, 0, 0, 128, 160, {0,-0.25}, "additive"),
            light = { color = {0.5,0.8,1.0}, intensity = 0.5, size = 8, shift = {0,0}},
            effect = "uranium-glow",
        },
    },
    working_sound = {
        sound = {
            filename = "__base__/sound/accumulator-working.ogg",
            volume = 1.0
        },
        idle_sound = {
            filename = "__base__/sound/idle1.ogg",
            volume = 0.6
        },
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
    tile_width = 2,
    tile_height = 2,
	match_animation_speed_to_activity = false,
}})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- battery discharger

data:extend({{
    name = "iron-battery-discharger",
    type = "generator",
    placeable_by = {item = "iron-battery-discharger", count = 1},
    minable = {
        mining_time = 0.5,
        result = "iron-battery-discharger"
    },
    order = "z-iron-battery-discharger",
    icon = get_icon_path("iron-battery-discharger", DIR.icon_size),
    icon_size = DIR.icon_size,
    effectivity = 1,
    energy_source = {
        type = "electric",
        usage_priority = "tertiary",
        drain = "0kW",
    },
    max_power_output = "500kW",
    burner = {
        emissions_per_minute = 0,
        fuel_category = "battery",
        fuel_inventory_size = 1,
        burnt_inventory_size = 1,
        type = "burner"
    },
    max_health = 100,
    dying_explosion = "medium-explosion",
    resistances = {
        {
            type = "fire",
            percent = 80
        },
    },
    corpse = "medium-remnants",
    flags = {"placeable-player", "placeable-neutral", "player-creation"},
    collision_box = { {-0.9,-0.9}, {0.9,0.9} },
    selection_box = { {-1.0,-1.0}, {1.0,1.0} },
    horizontal_animation = {
        layers = {
            get_layer("machines/iron-battery-discharger-base", 30, 6, false, nil, DIR.anim_speed, 128, 160, 0, 0, 128, 160, {0,-0.25}),
            get_layer("machines/iron-battery-discharger-shadow", 30, 5, true, nil, DIR.anim_speed, 256, 160, 0, 0, 256, 160, {1.0,-0.25}),
        }
    },
    vertical_animation = {
        layers = {
            get_layer("machines/iron-battery-discharger-base", 30, 6, false, nil, DIR.anim_speed, 128, 160, 0, 0, 128, 160, {0,-0.25}),
            get_layer("machines/iron-battery-discharger-shadow", 30, 5, true, nil, DIR.anim_speed, 256, 160, 0, 0, 256, 160, {1.0,-0.25}),
        }
    },
    working_sound = {
        sound = {
            filename = "__base__/sound/accumulator-working.ogg",
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
    show_recipe_icon = false,
}})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- large solar array

data:extend({{
    name = "solar-array",
    type = "solar-panel",
    collision_box = {{-2.4,-2.4},{2.4,2.4}},
    selection_box = {{-2.5,-2.5},{2.5,2.5}},
    corpse = "big-remnants",
    energy_source = {
        type = "electric",
        usage_priority = "solar"
    },
    flags = {
        "placeable-neutral",
        "player-creation"
    },
    icon = get_icon_path("solar-array", DIR.icon_size),
    icon_size = DIR.icon_size,
    max_health = 800,
    minable = {
        mining_time = 0.5,
        result = "solar-array"
    },
    picture = {
        layers = {
            get_layer("machines/solar-array-base", 1, nil, false, nil, nil, 384, 384, 0, 0, 384, 384, {0.5,-0.5}),
            get_layer("machines/solar-array-shadow", 1, nil, true, nil, nil, 384, 384, 0, 0, 384, 384, {0.5,-0.5}),
        }
    },
    overlay = {
        layers = {
            get_layer("machines/solar-array-overlay", 1, nil, false, nil, nil, 384, 384, 0, 0, 384, 384, {0.5,-0.5}),
        }
    },
    production = "300kW",
    vehicle_impact_sound = {
        filename = "__base__/sound/car-metal-impact.ogg",
        volume = 0.65
    }
}})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- tweak solar panel

local solar = data.raw["solar-panel"]["solar-panel"]
if solar then
    solar.picture = {
        layers = {
            get_layer("machines/solar-panel-base", 1, nil, false, nil, nil, 256, 256, 0, 0, 256, 256, {0.5,-0.5}),
            get_layer("machines/solar-panel-shadow", 1, nil, true, nil, nil, 256, 256, 0, 0, 256, 256, {0.5,-0.5}),
        }
    }
    solar.overlay = {
        layers = {
            get_layer("machines/solar-panel-overlay", 1, nil, false, nil, nil, 256, 256, 0, 0, 256, 256, {0.5,-0.5}),
        }
    }
    solar.production = "75kW"
    solar.icon = get_icon_path("solar-panel", DIR.icon_size)
    solar.icon_size = DIR.icon_size
end
solar = data.raw.item["solar-panel"]
if solar then
    solar.icon = get_icon_path("solar-panel", DIR.icon_size)
    solar.icon_size = DIR.icon_size
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- big wooden pole

data:extend({
    {
        type = "electric-pole",
        name = "big-wooden-pole",
        icon = get_icon_path("big-wooden-pole"),
        icon_size = DIR.icon_size,
        flags = {"placeable-neutral", "player-creation", "fast-replaceable-no-build-while-moving"},
        minable = {mining_time = 0.1, result = "big-wooden-pole"},
        max_health = 150,
        corpse = "medium-remnants",
        resistances =
        {
            {
                type = "fire",
                percent = 50
            }
        },
        collision_box = {{-0.55, -0.55}, {0.55, 0.55}},
        selection_box = {{-1, -1}, {1, 1}},
        drawing_box = {{-1, -3}, {1, 0.5}},
        maximum_wire_distance = 20,
        supply_area_distance = 2,
        vehicle_impact_sound =  { filename = "__base__/sound/car-metal-impact.ogg", volume = 0.65 },
        pictures =
        {
            layers =
            {
				 get_layer("machines/big-wooden-pole-base", 1, 4, false, nil, nil, 128, 384, 0, 0, 128, 384, {0,-2}, nil, nil, nil, 4),
				 get_layer("machines/big-wooden-pole-shadow", 1, 1, true, nil, nil, 512, 128, 0, 0, 512, 128, {3,0}, nil, nil, nil, 4),
            }
        },
        connection_points =
        {
            {
                shadow =
                {
                    copper = util.by_pixel_hr(328,6),
                    red = util.by_pixel_hr(366, 6),
                    green = util.by_pixel_hr(258, 6)
                },
                wire =
                {
                    copper = util.by_pixel_hr(0, -232),
                    red = util.by_pixel_hr(52, -214),
                    green = util.by_pixel_hr(-52, -214)
                }
            },
            {
                shadow =
                {
                    copper = util.by_pixel_hr(328,6),
                    red = util.by_pixel_hr(348, 34),
                    green = util.by_pixel_hr(272, -20)
                },
                wire =
                {
                    copper = util.by_pixel_hr(0, -232),
                    red = util.by_pixel_hr(37, -187),
                    green = util.by_pixel_hr(-37, -241)
                }
            },
            {
                shadow =
                {
                    copper = util.by_pixel_hr(328,6),
                    red = util.by_pixel_hr(311, 45),
                    green = util.by_pixel_hr(311, -30)
                },
                wire =
                {
                    copper = util.by_pixel_hr(0, -232),
                    red = util.by_pixel_hr(0, -176),
                    green = util.by_pixel_hr(0, -216)
                }
            },
            {
                shadow =
                {
                    copper = util.by_pixel_hr(328,6),
                    red = util.by_pixel_hr(348, -20),
                    green = util.by_pixel_hr(272, 34)
                },
                wire =
                {
                    copper = util.by_pixel_hr(0, -238),
                    red = util.by_pixel_hr(37, -241),
                    green = util.by_pixel_hr(-37, -197)
                }
            }
        },
        radius_visualisation_picture =
        {
            filename = "__base__/graphics/entity/small-electric-pole/electric-pole-radius-visualization.png",
            width = 12,
            height = 12,
            priority = "extra-high-no-scale"
        }
    }
})


------------------------------------------------------------------------------------------------------------------------------------------------------
