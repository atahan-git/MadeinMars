------------------------------------------------------------------------------------------------------------------------------------------------------

-- LABS

------------------------------------------------------------------------------------------------------------------------------------------------------

-- vanilla

local speed = 1

data.raw.lab["lab"].fast_replaceable_group = "lab"

local energy = get_scaled_energy_usage(speed, 0.04)
data.raw.lab["lab"].energy_source = {
	type = "electric",
	usage_priority = "secondary-input",
	drain = energy.passive,
}
data.raw.lab["lab"].energy_usage = energy.active

------------------------------------------------------------------------------------------------------------------------------------------------------

-- copper lab

local animspeed = DIR.anim_speed / speed
local energy = get_scaled_energy_usage(speed, 0)
local lab = {
    name = "copper-lab",
    type = "lab",
    collision_box = {{-1.2, -1.2}, {1.2,1.2}},
    corpse = "big-remnants",
    dying_explosion = "medium-explosion",
    energy_source = {
        effectivity = 1,
        emissions_per_minute = 3,
        fuel_category = "chemical",
        fuel_inventory_size = 1,
        type = "burner"
    },
    energy_usage = energy.active,
    fast_replaceable_group = "lab",
    flags = {"placeable-player", "placeable-neutral", "player-creation"},
    icon = get_icon_path("copper-lab", DIR.icon_size),
    icon_size = DIR.icon_size,
    inputs = {
        "automation-science-pack",
        "logistic-science-pack",
    },
    light = {
        color = {r=1, g=0.75, b=0},
        intensity = 0.75,
        size = 8
    },
    max_health = 100,
    minable = {
        mining_time = 0.2,
        result = "copper-lab"
    },
    off_animation = {
        layers = {
            get_layer("machines/copper-lab-base", nil, nil, false, nil, animspeed, 192, 192, 0, 0, 192, 192, {0,0}),
            get_layer("machines/copper-lab-shadow", nil, nil, true, nil, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
        }
    },
    on_animation = {
        layers = {
            get_layer("machines/copper-lab-base", 1, nil, false, 30, animspeed, 192, 192, 0, 0, 192, 192, {0,0}),
            get_layer("machines/copper-lab-shadow", 1, nil, true, 30, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
            get_layer("machines/copper-lab-working", 30, 5, false, nil, animspeed, 192, 192, 0, 0, 192, 192, {0,0}, "additive"),
        }
    },
    researching_speed = speed,
    selection_box = {{-1.5,-1.5}, {1.5,1.5}},
    vehicle_impact_sound = {
        filename = "__base__/sound/car-metal-impact.ogg",
        volume = 0.65
    },
    working_sound = {
        sound = {
            filename = "__base__/sound/furnace.ogg",
            volume = 1.0
        }
    }
}
data:extend({lab})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- quantum lab

local speed = 12
local anim_speed = 0.5
local energy = get_scaled_energy_usage(10, 0.04)
local lab = {
    name = "quantum-lab",
    type = "lab",
    collision_box = {{-3.25,-3.25}, {3.25,3.25}},
    selection_box = {{-3.5,-3.5}, {3.5,3.5}},
	drawing_box = { {-3.5,-4.5}, {3.5,3.5} },
    corpse = "big-remnants",
    dying_explosion = "big-artillery-explosion",
    dying_trigger_effect = {
        {
            repeat_count = 100,
            type = "create-trivial-smoke",
            smoke_name = "nuclear-smoke",
            offset_deviation = {{-1, -1}, {1, 1}},
            starting_frame = 3,
            starting_frame_deviation = 5,
            starting_frame_speed = 0,
            starting_frame_speed_deviation = 5,
            speed_from_center = 0.5
        },
        {
            type = "create-entity",
            entity_name = "explosion"
        },
        {
            type = "damage",
            damage = {amount = 400, type = "explosion"}
        },
        {
            type = "create-entity",
            entity_name = "small-scorchmark",
            check_buildability = true
        },
        {
            type = "nested-result",
            action =
            {
                type = "area",
                target_entities = false,
                trigger_from_target = true,
                repeat_count = 2000,
                radius = 25,
                action_delivery =
                {
                    type = "projectile",
                    projectile = "atomic-bomb-wave",
                    starting_speed = 0.5
                }
            }
        }
    },
    energy_source = {
        type = "electric",
        usage_priority = "secondary-input",
        drain = energy.passive,
    },
    energy_usage = energy.active,
    flags = {"placeable-player", "placeable-neutral", "player-creation"},
    icon = get_icon_path("quantum-lab", DIR.icon_size),
    icon_size = DIR.icon_size,
    inputs = data.raw.lab["lab"].inputs,
    light = {
        color = {r=0.9, g=0.9, b=1.0},
        intensity = 0.9,
        size = 20
    },
    max_health = 1250,
    minable = {
        mining_time = 0.2,
        result = "quantum-lab"
    },
    module_specification = {
        max_entity_info_module_icon_rows = 1,
        max_entity_info_module_icons_per_row = 4,
        module_info_icon_shift = { 0, 2 },
        module_slots = 4
    },
    off_animation = {
        layers = {
            get_layer("machines/quantum-lab-base-shadow", 1, nil, true, nil, anim_speed, 768, 448, 0, 0, 768, 448, {2.5,0}),
            get_layer("machines/quantum-lab-rings-shadow", 1, nil, true, nil, anim_speed, 512, 384, 0, 0, 512, 384, {4.5,0.5}),
            get_layer("machines/quantum-lab-base", 1, nil, false, nil, anim_speed, 448, 512, 0, 0, 448, 512, {0,-0.5}),
            get_layer("machines/quantum-lab-rings-off", 1, nil, false, nil, anim_speed, 448, 448, 0, 0, 448, 448, {0,-2}),
        }
    },
    on_animation = {
        layers = {
			get_layer("machines/quantum-lab-base-shadow", 1, nil, true, 60, anim_speed, 768, 448, 0, 0, 768, 448, {2.5,0}),
			get_layer("machines/quantum-lab-rings-shadow", 60, 6, true, nil, anim_speed, 512, 384, 0, 0, 512, 384, {4.5,0.5}),
			get_layer("machines/quantum-lab-base", 1, nil, false, 60, anim_speed, 448, 512, 0, 0, 448, 512, {0,-0.5}),
			get_layer("machines/quantum-lab-rings-on", 60, 10, false, nil, anim_speed, 448, 448, 0, 0, 448, 448, {0,-2}),
			get_layer("machines/quantum-lab-lights-1", 60, 10, false, nil, anim_speed, 64, 256, 0, 0, 64, 256, {-3,1.5}),
			get_layer("machines/quantum-lab-lights-2", 60, 10, false, nil, anim_speed, 64, 256, 0, 0, 64, 256, {-1.5,1.5}),
			get_layer("machines/quantum-lab-lights-3", 60, 10, false, nil, anim_speed, 64, 256, 0, 0, 64, 256, {1,1.5}),
			get_layer("machines/quantum-lab-lights-4", 60, 10, false, nil, anim_speed, 64, 256, 0, 0, 64, 256, {3,1.5}),
        }
    },
    researching_speed = speed,
    vehicle_impact_sound = {
        filename = "__base__/sound/car-metal-impact.ogg",
        volume = 0.65
    },
    working_sound = {
        sound = {
            filename = DIR.sound_path.."/quantum.ogg",
            volume = 1.5
        }
    },
}
data:extend({lab})
