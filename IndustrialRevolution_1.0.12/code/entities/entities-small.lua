------------------------------------------------------------------------------------------------------------------------------------------------------

-- SMALL MACHINES

------------------------------------------------------------------------------------------------------------------------------------------------------

-- incinerator template

data:extend({{
    type = "recipe-category",
    name = "incinerating",
}})

local incinerator = {
    type = "furnace",
    order = "z-incinerator",
    icon_size = DIR.icon_size,
    crafting_speed = 1,
    source_inventory_size = 1,
    result_inventory_size = 1,
    show_recipe_icon = false,
    energy_usage = get_scaled_energy_usage(0.5,0).active,
    crafting_categories = {"incinerating"},
    max_health = 120,
    dying_explosion = "explosion",
    resistances = {
        {
            type = "fire",
            percent = 95
        },
    },
    corpse = "small-remnants",
    flags = {"placeable-player", "placeable-neutral", "player-creation"},
    collision_box = { {-0.4,-0.4}, {0.4,0.4} },
    selection_box = { {-0.5,-0.5}, {0.5,0.5} },
    open_sound = {
        filename = "__base__/sound/machine-open.ogg",
        volume = 0.75
    },
    close_sound = {
        filename = "__base__/sound/machine-close.ogg",
        volume = 0.75
    },
    mined_sound = {
        filename = "__core__/sound/deconstruct-small.ogg"
    },
    vehicle_impact_sound = {
        filename = "__base__/sound/car-metal-impact.ogg",
        volume = 0.65
    },
    working_sound = {
        sound = {
            filename = "__base__/sound/furnace.ogg",
            volume = 1.0
        }
    },
}

-- copper incinerator

local copper = table.deepcopy(incinerator)

copper.name = "copper-incinerator"
copper.icon = get_icon_path("copper-incinerator", DIR.icon_size)
copper.placeable_by = {item = "copper-incinerator", count = 1}
copper.minable = {
    mining_time = 0.5,
    result = "copper-incinerator"
}
copper.energy_source = {
    effectivity = 1,
    emissions_per_minute = 30,
    fuel_category = "chemical",
    fuel_inventory_size = 1,
    smoke = {
        {
            frequency = 12,
            name = "smoke",
            position = {
                0,
                -0.45
            },
            starting_frame_deviation = 60,
            starting_vertical_speed = 0.08
        },
    },
    type = "burner"
}
copper.animation = {
    layers = {
        get_layer("machines/copper-incinerator-base", 1, 1, false, nil, 1, 96, 96, 0, 0, 96, 96, {0,-0.25}),
        get_layer("machines/incinerator-shadow", 1, 1, true, nil, 1, 128, 96, 0, 0, 128, 96, {0.25,-0.125}),
    }
}
copper.working_visualisations = {
    {
        animation = get_layer("machines/incinerator-working", 30, 5, false, nil, 1, 96, 96, 0, 0, 96, 96, {0,-0.25}, "additive"),
    },
}

data:extend({copper})

-- iron incinerator

local iron = table.deepcopy(incinerator)
local energy = get_scaled_energy_usage(0.5, 0.05)

iron.name = "iron-incinerator"
iron.icon = get_icon_path("iron-incinerator", DIR.icon_size)
iron.placeable_by = {item = "iron-incinerator", count = 1}
iron.minable = {
    mining_time = 0.5,
    result = "iron-incinerator"
}
iron.energy_usage = energy.active
iron.energy_source = {
    type = "electric",
    usage_priority = "secondary-input",
    drain = energy.passive,
    emissions_per_minute = 30,
}
iron.animation = {
    layers = {
        get_layer("machines/iron-incinerator-base", 1, 1, false, nil, 1, 96, 96, 0, 0, 96, 96, {0,-0.25}),
        get_layer("machines/incinerator-shadow", 1, 1, true, nil, 1, 128, 96, 0, 0, 128, 96, {0.25,-0.125}),
    }
}
iron.working_visualisations = {
    {
        animation = get_layer("machines/incinerator-working", 30, 5, false, nil, 1, 96, 96, 0, 0, 96, 96, {0,-0.25}, "additive"),
        light = { color = {1.0,0.75,0.25}, intensity = 0.5, size = 6, shift = {0,0}},
        effect = "uranium-glow",
    },
}

data:extend({iron})

------------------------------------------------------------------------------------------------------------------------------------------------------
