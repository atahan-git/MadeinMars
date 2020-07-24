------------------------------------------------------------------------------------------------------------------------------------------------------

-- BOTS

------------------------------------------------------------------------------------------------------------------------------------------------------

-- buff vanilla bots

data.raw["construction-robot"]["construction-robot"].speed = 0.075
data.raw["construction-robot"]["construction-robot"].speed_multiplier_when_out_of_energy = 0.25
data.raw["logistic-robot"]["logistic-robot"].speed_multiplier_when_out_of_energy = 0.25

------------------------------------------------------------------------------------------------------------------------------------------------------

-- change defender bot to physical projectile

local ammo_material = DIR.materials["steel"]
data.raw["combat-robot"]["defender"].attack_parameters.ammo_type = get_bullet_ammo_type(ammo_material.base_damage, ammo_material.hue, ammo_material.saturation)
data.raw["combat-robot"]["defender"].attack_parameters.range = DIR.ammo.range.magazine

------------------------------------------------------------------------------------------------------------------------------------------------------

-- roboport buffer nerf

data.raw.roboport["roboport"].energy_source.buffer_capacity = "60MJ"
data.raw.roboport["roboport"].recharge_minimum = "30MJ"

------------------------------------------------------------------------------------------------------------------------------------------------------

-- clockwork punkbot

local idle = get_layer("bots/steambot", 1, 16, false, nil, 0.5, 80, 80, 0, 0, 80, 80, {0,0}, nil, nil, nil, 16)
local working = get_layer("bots/steambot-working", 1, 16, false, nil, 0.5, 80, 80, 0, 0, 80, 80, {0,0}, nil, nil, nil, 16)
local shadow = get_layer("bots/steambot-shadow", 1, 16, true, nil, 0.5, 128, 80, 0, 0, 128, 80, {1.05,0.59}, nil, nil, nil, 16)

local steambot = {
    cargo_centered = {0,0.2},
    collision_box = {{0,0},{0,0}},
    selection_box = {{-0.5,-1.5},{0.5,-0.5}},
    construction_vector = {0.3,0.22},
    draw_cargo = true,
    energy_per_move = "2.5kJ",
    energy_per_tick = "0.025kJ",
    flags = {
        "placeable-player",
        "player-creation",
        "placeable-off-grid",
        "not-on-map"
    },
    icon = get_icon_path("steambot",DIR.icon_size),
    icon_size = DIR.icon_size,
    idle = idle,
    in_motion = idle,
    working = working,
    shadow_idle = shadow,
    shadow_in_motion = shadow,
    shadow_working = shadow,
    max_energy = "0.75MJ",
    max_health = 75,
    max_payload_size = 1,
    max_to_charge = 0.95,
    min_to_charge = 0.2,
    minable = {
        mining_time = 0.1,
        result = "steambot"
    },
    name = "steambot",
    resistances = {{percent = 85, type = "fire"}},
    smoke = data.raw["construction-robot"]["construction-robot"].smoke,
    sparks = data.raw["construction-robot"]["construction-robot"].sparks,
    speed = 0.06,
    speed_multiplier_when_out_of_energy = 0.25,
    max_speed = 0.06,
    transfer_distance = 0.5,
    type = "construction-robot",
    working_light = {
        color = {
            b = 0.4,
            g = 0.7,
            r = 0.8
        },
        intensity = 0.8,
        size = 3
    },
    working_sound = {
        max_sounds_per_type = 3,
        sound = {
            {
                filename = DIR.sound_path.."/ticktock.ogg",
                volume = 0.8
            },
        }
    },
}

data:extend({steambot})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- 2x2 robotower

local anim_speed = 1/30
local circuits = circuit_connector_definitions.create (
    universal_connector_template,
    {{ variation = 18, main_offset = util.by_pixel(0, -14), shadow_offset = util.by_pixel(10, -4), show_shadow = true }}
)

data:extend({
    {
        name = "robotower",
        minable = {
            mining_time = 0.1,
            result = "robotower"
        },
        base = {
            layers = {
                get_layer("machines/robotower-base", 1, 1, false, nil, nil, 128, 192, 0, 0, 128, 192, {0,-0.5}),
                get_layer("machines/robotower-shadow", 1, 1, true, nil, nil, 256, 192, 0, 0, 256, 192, {1,0.5}),
            },
        },
        base_animation = get_layer("machines/robotower-lights", 16, 16, false, nil, anim_speed, 64, 96, 0, 0, 64, 96, {0,0.25}),
        base_patch = get_layer("machines/robotower-patch", 1, 1, false, nil, nil, 128, 32, 0, 0, 128, 32, {0,-0.75}),
        door_animation_down = get_layer("machines/robotower-door-down", 16, 16, false, nil, nil, 96, 48, 0, 0, 96, 48, {0,-0.625}),
        door_animation_up = get_layer("machines/robotower-door-up", 16, 16, false, nil, nil, 96, 48, 0, 0, 96, 48, {0,-1.375}),
        charge_approach_distance = 4,
        charging_energy = "1MW",
        charging_offsets = {{-0.85,-0.5},{0.85,-0.5}},
        spawn_and_station_height = 0.5,
		stationing_offset = {0,-0.5},
        circuit_connector_sprites = circuits.sprites,
        circuit_wire_connection_point = circuits.points,
        circuit_wire_max_distance = 9,
		robots_shrink_when_entering_and_exiting = true,
        close_door_trigger_effect = {
            {
                sound = {
                    filename = "__base__/sound/roboport-door.ogg",
                    volume = 0.75
                },
                type = "play-sound"
            }
        },
        collision_box = {{-0.95,-0.95},{0.95,0.95}},
        selection_box = {{-1,-1},{1,1}},
        construction_radius = 48,
        corpse = "medium-remnants",
        default_available_construction_output_signal = {
            name = "signal-Z",
            type = "virtual"
        },
        default_available_logistic_output_signal = {
            name = "signal-X",
            type = "virtual"
        },
        default_total_construction_output_signal = {
            name = "signal-T",
            type = "virtual"
        },
        default_total_logistic_output_signal = {
            name = "signal-Y",
            type = "virtual"
        },
        draw_construction_radius_visualization = true,
        draw_logistic_radius_visualization = true,
        dying_explosion = "medium-explosion",
        energy_source = {
            buffer_capacity = "20MJ",
            input_flow_limit = "2.5MW",
            type = "electric",
            usage_priority = "secondary-input"
        },
        energy_usage = "25kW",
        flags = {
            "placeable-player",
            "player-creation"
        },
        icon = get_icon_path("robotower"),
        icon_size = DIR.icon_size,
        logistics_radius = 16,
        material_slots_count = 3,
        max_health = 250,
        open_door_trigger_effect = {
            {
                sound = {
                    filename = "__base__/sound/roboport-door.ogg",
                    volume = 1
                },
                type = "play-sound"
            }
        },
        recharge_minimum = "10MJ",
        recharging_animation = {
            animation_speed = 0.5,
            filename = "__base__/graphics/entity/roboport/roboport-recharging.png",
            frame_count = 16,
            height = 35,
            priority = "high",
            scale = 1.5,
            width = 37
        },
        recharging_light = {
            color = {
                b = 1,
                g = 0.75,
                r = 0.75
            },
            intensity = 0.4,
            size = 5
        },
        request_to_open_door_timeout = 15,
        resistances = {
            {
                percent = 60,
                type = "fire"
            },
            {
                percent = 30,
                type = "impact"
            }
        },
        robot_slots_count = 3,
        type = "roboport",
        vehicle_impact_sound = {
            filename = "__base__/sound/car-metal-impact.ogg",
            volume = 0.65
        },
        working_sound = {
            audible_distance_modifier = 0.5,
            max_sounds_per_type = 3,
            probability = 0.003333333333333333,
            sound = {
                filename = "__base__/sound/roboport-working.ogg",
                volume = 0.6
            }
        }
    }
})
  
------------------------------------------------------------------------------------------------------------------------------------------------------
