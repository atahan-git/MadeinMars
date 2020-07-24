------------------------------------------------------------------------------------------------------------------------------------------------------

-- ASSEMBLERS

------------------------------------------------------------------------------------------------------------------------------------------------------

-- assembler 1

local speed = 0.5
local animspeed = DIR.anim_speed -- not scaled by activity
local energy = get_scaled_energy_usage(speed, 0)

local assembler = data.raw["assembling-machine"]["assembling-machine-1"]

assembler.crafting_speed = speed
assembler.crafting_categories = {"crafting"}
assembler.energy_source = {
    effectivity = 1,
    emissions_per_minute = 3,
    fuel_category = "chemical",
    fuel_inventory_size = 1,
    type = "burner"
}
assembler.energy_usage = energy.active

assembler.max_health = 250

assembler.allowed_effects = {
    "pollution"
}

assembler.module_specification = {
    module_info_icon_shift = standard_entity_info_module_shift(),
    module_slots = 1
}

assembler.animation = {
    layers = {
        get_layer("machines/assembler1-base", 60, 10, false, nil, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}),
        get_layer("machines/copper-shadow", nil, nil, true, 60, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
        get_layer("machines/assembler1-shadow", 60, 10, true, nil, animspeed, 128, 192, 0, 0, 128, 192, {2.5,0}),
    }
}

assembler.working_visualisations = {
    {
        animation = get_layer("machines/assembler1-working-glow", 60, 10, false, nil, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}, "additive"),
    },
}

assembler.working_sound = {
    apparent_volume = 1.5,
    idle_sound = {
        filename = "__base__/sound/idle1.ogg",
        volume = 0.6
    },
    sound = {
        {
            filename = string.format("%s/%s.ogg", DIR.sound_path, "rhythmic"),
            volume = 1.0
        },
    }
}

assembler.icon = get_icon_path("assembler1", DIR.icon_size)
assembler.icon_size = DIR.icon_size
assembler.entity_info_icon_shift = standard_entity_info_icon_shift()
assembler.collision_box = standard_3x3_collision()
assembler.selection_box = standard_3x3_selection()
assembler.match_animation_speed_to_activity = false

------------------------------------------------------------------------------------------------------------------------------------------------------

-- assembler 2

speed = 1
energy = get_scaled_energy_usage(speed, 0.04)

assembler = data.raw["assembling-machine"]["assembling-machine-2"]

assembler.crafting_speed = speed
assembler.crafting_categories = {"crafting","advanced-crafting","crafting-with-fluid"}
assembler.energy_usage = energy.active
assembler.energy_source = {
    type = "electric",
    usage_priority = "secondary-input",
    drain = energy.passive,
    emissions_per_minute = 1.5,
}

assembler.max_health = 400

assembler.module_specification = {
    module_info_icon_shift = standard_entity_info_module_shift(),
    module_slots = 2
}

assembler.animation = {
    layers = {
        get_layer("machines/assembler2-base", 60, 10, false, nil, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}),
        get_layer("machines/iron-shadow", nil, nil, true, 60, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
    }
}

assembler.working_visualisations = {
    {
        animation = get_layer("machines/assembler2-working-sparks", 60, 10, false, nil, animspeed, 192, 192, 0, 0, 192, 192, {0,-1}, "additive"),
    },
    {
        animation = get_layer("machines/assembler-glow", 30, 5, false, nil, animspeed, 192, 128, 0, 0, 192, 128, {0,0}, "additive", nil, {r=0.5,g=1.0,b=1.0}),
        light = { color = DIR.indicator_colours.iron, intensity = 0.5, size = 9, shift = {0,0.5} },
    },
}

assembler.working_sound = {
    apparent_volume = 1.5,
    idle_sound = {
        filename = "__base__/sound/idle1.ogg",
        volume = 0.6
    },
    sound = {
        {
            filename = string.format("%s/%s.ogg", DIR.sound_path, "saw-weld"),
            volume = 1.0
        },
    }
}

assembler.fluid_boxes = standard_fluid_boxes()
assembler.icon = get_icon_path("assembler2", DIR.icon_size)
assembler.icon_size = DIR.icon_size
assembler.entity_info_icon_shift = standard_entity_info_icon_shift()
assembler.collision_box = standard_3x3_collision()
assembler.selection_box = standard_3x3_selection()
assembler.match_animation_speed_to_activity = false

------------------------------------------------------------------------------------------------------------------------------------------------------

-- assembler 3

speed = 2
energy = get_scaled_energy_usage(2.5, 0.04)

assembler = data.raw["assembling-machine"]["assembling-machine-3"]

assembler.crafting_speed = speed
assembler.crafting_categories = {"crafting","advanced-crafting","crafting-with-fluid","endgame-crafting"}
assembler.energy_usage = energy.active
assembler.energy_source = {
    type = "electric",
    usage_priority = "secondary-input",
    drain = energy.passive,
    emissions_per_minute = 1,
}

assembler.max_health = 600

assembler.module_specification = {
    module_info_icon_shift = standard_entity_info_module_shift(),
    module_slots = 4
}

assembler.icon = get_icon_path("assembler3", DIR.icon_size)
assembler.icon_size = DIR.icon_size
assembler.default_recipe_tint = get_crafting_tints_from_hue(1,1)
assembler.animation = {
    layers = {
        get_layer("machines/assembler3-base", 60, 10, false, nil, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}),
        get_layer("machines/iron-shadow", nil, nil, true, 60, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
        get_layer("machines/assembler3-shadow", 60, 10, true, nil, animspeed, 96, 96, 0, 0, 96, 96, {2.75,0.25}),
    }
}
assembler.working_visualisations = {
    {
        animation = get_layer("machines/assembler-glow", 30, 5, false, nil, animspeed, 192, 128, 0, 0, 192, 128, {0,0}, "additive", nil, DIR.indicator_colours.steel),
    },
    {
        animation = get_layer("machines/assembler3-working-sparks", 60, 10, false, nil, animspeed, 192, 192, 0, 0, 192, 192, {0,-1}, "additive"),
    },
    {
        animation = get_layer("machines/assembler3-working-tinted", 60, 10, false, nil, animspeed, 192, 192, 0, 0, 192, 192, {0,-1}, "additive"),
        apply_recipe_tint = "primary",
    },
    {
        animation = get_layer("machines/assembler3-working-white", 60, 10, false, nil, animspeed, 192, 192, 0, 0, 192, 192, {0,-1}, "additive"),
    },
    {
        animation = get_layer("machines/steel-working-indicator", 20, 5, false, nil, animspeed, 64, 64, 0, 0, 64, 64, {0,1}, "additive"),
        apply_recipe_tint = "primary",
    },
    {
        light = { color = {1,1,1}, intensity = 0.5, size = 9, shift = {0,0.5}},
        apply_recipe_tint = "secondary",
    },
}

assembler.working_sound = {
    apparent_volume = 1.5,
    idle_sound = {
        filename = "__base__/sound/idle1.ogg",
        volume = 0.6
    },
    sound = {
        {
            filename = string.format("%s/%s.ogg", DIR.sound_path, "high-tech-industrial"),
            volume = 1.0
        },
    }
}

assembler.fluid_boxes = standard_fluid_boxes()
assembler.entity_info_icon_shift = standard_entity_info_icon_shift()
assembler.collision_box = standard_3x3_collision()
assembler.selection_box = standard_3x3_selection()
assembler.match_animation_speed_to_activity = false

------------------------------------------------------------------------------------------------------------------------------------------------------
