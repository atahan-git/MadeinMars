------------------------------------------------------------------------------------------------------------------------------------------------------

-- TURRETS

------------------------------------------------------------------------------------------------------------------------------------------------------

-- resistances
local resistances = {
	{
		type = "fire",
		percent = 90
	},
	{
		type = "acid",
		percent = 75
	},
	{
		type = "impact",
		percent = 50
	},
}

-- gun turret

local gun_turret = data.raw["ammo-turret"]["gun-turret"]
gun_turret.fast_replaceable_group = "turret"
gun_turret.attack_parameters.projectile_center = {0,-0.25}
gun_turret.attack_parameters.projectile_creation_distance = 1.4
gun_turret.attack_parameters.lead_target_for_projectile_speed = DIR.projectile_speed.bullet
gun_turret.icon = get_icon_path("autogun-turret", DIR.icon_size)
gun_turret.icon_size = DIR.icon_size
gun_turret.prepare_range = DIR.ammo.range.magazine + 5
gun_turret.resistances = resistances
gun_turret.entity_info_icon_shift = standard_2x2_turret_info_icon_shift()
gun_turret.attacking_speed = 1/3
gun_turret.hide_resistances = false

gun_turret.base_picture = {
    layers = {
        get_layer("turrets/iron-ammo-turret-base", 1, 1, false, nil, nil, 128, 192, 0, 0, 128, 192, {0,-0.5}, nil, nil, nil, nil),
        get_layer("turrets/iron-ammo-turret-base-mask", 1, 1, false, nil, nil, 128, 192, 0, 0, 128, 192, {0,-0.5}, nil, {"mask"}, nil, nil, true),
    }
}
gun_turret.attacking_animation = {
    layers = {
        get_multi_layer("turrets/autogun-turret-shooting-shadow", 4, 3, nil, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 64),
        get_multi_layer("turrets/autogun-turret-shooting-base", 4, 1, nil, false, 3, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
        get_multi_layer("turrets/autogun-turret-shooting-mask", 4, 1, nil, false, 3, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 64, true),
        get_multi_layer("turrets/autogun-turret-shooting-gun", 4, 3, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
    }
}
gun_turret.prepared_animation = {
    layers = {
        get_multi_layer("turrets/autogun-turret-shooting-shadow", 4, 1, nil, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 64),
        get_multi_layer("turrets/autogun-turret-shooting-base", 4, 1, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
        get_multi_layer("turrets/autogun-turret-shooting-mask", 4, 1, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 64, true),
        get_multi_layer("turrets/autogun-turret-shooting-gun", 4, 1, nil, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
    }
}
gun_turret.folded_animation = {
    layers = {
        get_layer("turrets/autogun-turret-raising-shadow", 1, 1, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4),
        get_layer("turrets/autogun-turret-raising-base", 1, 1, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
        get_layer("turrets/autogun-turret-raising-mask", 1, 1, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true),
        get_layer("turrets/autogun-turret-raising-gun", 1, 1, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
    }
}
gun_turret.folding_animation = {
    layers = {
        get_layer("turrets/autogun-turret-raising-shadow", 5, 0, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4, false, "backward"),
        get_layer("turrets/autogun-turret-raising-base", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4, false, "backward"),
        get_layer("turrets/autogun-turret-raising-mask", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true, "backward"),
        get_layer("turrets/autogun-turret-raising-gun", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4, nil, "backward"),
    }
}
gun_turret.preparing_animation = {
    layers = {
        get_layer("turrets/autogun-turret-raising-shadow", 5, 0, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4),
        get_layer("turrets/autogun-turret-raising-base", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
        get_layer("turrets/autogun-turret-raising-mask", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true),
        get_layer("turrets/autogun-turret-raising-gun", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
    }
}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- copper scattergun turret

local scattergun_turret = table.deepcopy(gun_turret)

scattergun_turret.name = "scattergun-turret"
scattergun_turret.minable.result = "scattergun-turret"
scattergun_turret.icon = get_icon_path("scattergun-turret", DIR.icon_size)
scattergun_turret.icon_size = DIR.icon_size
scattergun_turret.attack_parameters.ammo_category = "shotgun-shell"
scattergun_turret.attack_parameters.range = DIR.ammo.range.cartridge
scattergun_turret.attack_parameters.cooldown = DIR.ammo.turret_speed.cartridge
scattergun_turret.attack_parameters.projectile_center = {0,-0.25}
scattergun_turret.attack_parameters.projectile_creation_distance = 1.5
scattergun_turret.attack_parameters.lead_target_for_projectile_speed = DIR.projectile_speed.shotgun
scattergun_turret.max_health = 325
scattergun_turret.prepare_range = DIR.ammo.range.cartridge + 5

scattergun_turret.base_picture = {
    layers = {
        get_layer("turrets/scattergun-turret-base", 1, 1, false, nil, nil, 128, 192, 0, 0, 128, 192, {0,-0.5}, nil, nil, nil, nil),
        get_layer("turrets/scattergun-turret-base-mask", 1, 1, false, nil, nil, 128, 192, 0, 0, 128, 192, {0,-0.5}, nil, {"mask"}, nil, nil, true),
    }
}
scattergun_turret.attacking_animation = {
    layers = {
        get_layer("turrets/scattergun-turret-shooting-shadow", 1, 8, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 64),
        get_layer("turrets/scattergun-turret-shooting", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
        get_layer("turrets/scattergun-turret-shooting-mask", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 64, true),
    }
}
scattergun_turret.prepared_animation = {
    layers = {
        get_layer("turrets/scattergun-turret-shooting-shadow", 1, 8, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 64),
        get_layer("turrets/scattergun-turret-shooting", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
        get_layer("turrets/scattergun-turret-shooting-mask", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 64, true),
    }
}
scattergun_turret.folded_animation = {
    layers = {
        get_layer("turrets/scattergun-turret-raising-shadow", 1, 1, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4),
        get_layer("turrets/scattergun-turret-raising", 1, 1, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
        get_layer("turrets/scattergun-turret-raising-mask", 1, 1, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true),
    }
}
scattergun_turret.folding_animation = {
    layers = {
        get_layer("turrets/scattergun-turret-raising-shadow", 5, 0, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4, false, "backward"),
        get_layer("turrets/scattergun-turret-raising", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4, nil, "backward"),
        get_layer("turrets/scattergun-turret-raising-mask", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true, "backward"),
    }
}
scattergun_turret.preparing_animation = {
    layers = {
        get_layer("turrets/scattergun-turret-raising-shadow", 5, 0, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4),
        get_layer("turrets/scattergun-turret-raising", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
        get_layer("turrets/scattergun-turret-raising-mask", 5, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true),
    }
}
scattergun_turret.attack_parameters.sound = {
    variations = {
		{
			filename = DIR.sound_path.."/shotgun1.ogg",
			volume = 0.5
		},
		{
			filename = DIR.sound_path.."/shotgun2.ogg",
			volume = 0.5
		},
		{
			filename = DIR.sound_path.."/shotgun3.ogg",
			volume = 0.5
		},
	}
}

data:extend({scattergun_turret})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- laser turret

local colour = DIR.laser_colours[settings.startup["deadlock-industry-laser-colour"].value].rgb
if not colour then colour = {1,0,0,1} end

data:extend({
    {
        name = "laser-turret",
        type = "electric-turret",
        attack_parameters = {
            ammo_type = {
                action = {
                    action_delivery = {
                        beam = "laser-beam",
                        duration = 40,
                        max_length = 24,
                        source_offset = {0,-1.175},
                        type = "beam"
                    },
                    type = "direct"
                },
                category = "laser-turret",
                energy_consumption = "800kJ"
            },
            cooldown = 20,
            damage_modifier = 2,
            range = 24,
            source_direction_count = 64,
            source_offset = {0,-0.9},
            type = "beam",
        },
        call_for_help_radius = 40,
        collision_box = {{-0.7,-0.7},{0.7,0.7}},
        selection_box = {{-1,-1},{1,1}},
        corpse = "medium-remnants",
        dying_explosion = "medium-explosion",
        energy_source = {
            buffer_capacity = "20MJ",
            drain = "25kW",
            -- input_flow_limit = "4MW",
            type = "electric",
            usage_priority = "primary-input"
        },
		entity_info_icon_shift = standard_2x2_turret_info_icon_shift(),
        fast_replaceable_group = "turret",
        flags = {
            "placeable-player",
            "placeable-enemy",
            "player-creation"
        },
        folding_speed = 0.05,
        glow_light_intensity = 0.75,
		hide_resistances = false,
        icon = get_icon_path("laser-turret", DIR.icon_size),
        icon_size = DIR.icon_size,
        max_health = 1000,
        minable = {
            mining_time = 0.5,
            result = "laser-turret"
        },
        preparing_speed = 0.05,
		prepare_range = 29,
		resistances = resistances,
        rotation_speed = 0.01,
        vehicle_impact_sound = {
            filename = "__base__/sound/car-metal-impact.ogg",
            volume = 0.65
        },
        base_picture = {
            layers = {
                get_layer("turrets/iron-turret-electric-base", 1, 1, false, nil, nil, 128, 192, 0, 0, 128, 192, {0,-0.5}, nil, nil, nil, nil),
                get_layer("turrets/iron-turret-electric-base-mask", 1, 1, false, nil, nil, 128, 192, 0, 0, 128, 192, {0,-0.5}, nil, {"mask"}, nil, nil, true),
            }
        },
        attacking_animation = {
            layers = {
                get_layer("turrets/laser-turret-shooting-shadow", 1, 8, true, 20, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 64),
                get_layer("turrets/laser-turret-shooting", 1, 8, false, 20, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
                get_layer("turrets/laser-turret-shooting-mask", 1, 8, false, 20, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 64, true),
            }
        },
        folded_animation = {
            layers = {
                get_layer("turrets/laser-turret-raising-shadow", 1, 1, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4),
                get_layer("turrets/laser-turret-raising", 1, 1, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
                get_layer("turrets/laser-turret-raising-mask", 1, 1, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true),
            }
        },
        folding_animation = {
            layers = {
                get_layer("turrets/laser-turret-raising-shadow", 16, 0, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4, false, "backward"),
                get_layer("turrets/laser-turret-raising", 16, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4, nil, "backward"),
                get_layer("turrets/laser-turret-raising-mask", 16, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true, "backward"),
            }
        },
        prepared_animation = {
            layers = {
                get_layer("turrets/laser-turret-shooting-shadow", 1, 8, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 64),
                get_layer("turrets/laser-turret-shooting", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
                get_layer("turrets/laser-turret-shooting-mask", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 64, true),
            }
        },
        preparing_animation = {
            layers = {
                get_layer("turrets/laser-turret-raising-shadow", 16, 0, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4),
                get_layer("turrets/laser-turret-raising", 16, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
                get_layer("turrets/laser-turret-raising-mask", 16, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true),
            }
        },
        energy_glow_animation = {
            layers = {
                get_layer("turrets/laser-turret-shooting-glow", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, "additive", {"light"}, colour, 64, true),
            }
        },
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- electric arc turret

data:extend({
    {
      bonus_gui_order = "kb",
      name = "emp-turret",
      type = "ammo-category"
    }
})

local arc_turret = table.deepcopy(data.raw["electric-turret"]["laser-turret"])

arc_turret.name = "arc-turret"
arc_turret.minable.result = "arc-turret"
arc_turret.icon = get_icon_path("arc-turret", DIR.icon_size)
arc_turret.icon_size = DIR.icon_size
arc_turret.max_health = 350
arc_turret.energy_source.buffer_capacity = "20MJ"
arc_turret.energy_source.drain = "50kW"
arc_turret.attack_parameters.source_offset = {0,-0.69375}
arc_turret.attack_parameters.ammo_type.action.action_delivery.source_offset = {0,-1.25}
arc_turret.attack_parameters.ammo_type.action.action_delivery.beam = "emp-beam"
arc_turret.attack_parameters.ammo_type.action.action_delivery.duration = 20
arc_turret.attack_parameters.ammo_type.action.action_delivery.max_length = 20
arc_turret.attack_parameters.ammo_type.category = "emp-turret"
arc_turret.attack_parameters.ammo_type.energy_consumption = "1600kJ"
arc_turret.attack_parameters.damage_modifier = 1
arc_turret.attack_parameters.range = 20
arc_turret.prepare_range = 25

arc_turret.attacking_animation = {
    layers = {
        get_layer("turrets/arc-turret-shooting-shadow", 1, 8, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 64),
        get_layer("turrets/arc-turret-shooting", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
        get_layer("turrets/arc-turret-shooting-mask", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 64, true),
    }
}
arc_turret.prepared_animation = {
    layers = {
        get_layer("turrets/arc-turret-shooting-shadow", 1, 8, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 64),
        get_layer("turrets/arc-turret-shooting", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 64),
        get_layer("turrets/arc-turret-shooting-mask", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 64, true),
    }
}
arc_turret.folded_animation = {
    layers = {
        get_layer("turrets/arc-turret-raising-shadow", 1, 1, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4),
        get_layer("turrets/arc-turret-raising", 1, 1, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
        get_layer("turrets/arc-turret-raising-mask", 1, 1, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true),
    }
}
arc_turret.folding_animation = {
    layers = {
        get_layer("turrets/arc-turret-raising-shadow", 16, 0, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4, false, "backward"),
        get_layer("turrets/arc-turret-raising", 16, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4, nil, "backward"),
        get_layer("turrets/arc-turret-raising-mask", 16, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true, "backward"),
    }
}
arc_turret.preparing_animation = {
    layers = {
        get_layer("turrets/arc-turret-raising-shadow", 16, 0, true, nil, nil, 256, 128, 0, 0, 256, 128, {1,0}, nil, nil, nil, 4),
        get_layer("turrets/arc-turret-raising", 16, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, nil, nil, 4),
        get_layer("turrets/arc-turret-raising-mask", 16, 0, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, nil, {"mask"}, nil, 4, true),
    }
}
arc_turret.energy_glow_animation = {
    layers = {
        get_layer("turrets/arc-turret-shooting-glow", 1, 8, false, nil, nil, 128, 128, 0, 0, 128, 128, {0,-1}, "additive", nil, {0.25,0.75,1,1}, 64, true),
    }
}

data:extend({arc_turret})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- photon turret

local photon_turret = table.deepcopy(data.raw["electric-turret"]["laser-turret"])

photon_turret.name = "photon-turret"
photon_turret.localised_name = {"entity-name.photon-turret"}
photon_turret.localised_description = {"entity-description.photon-turret"}
photon_turret.minable.result = "photon-turret"
photon_turret.placeable_by = {item = "photon-turret", count = 1}
photon_turret.icons = {
	{ icon = get_icon_path("photon-turret"), icon_size = DIR.icon_size },
	{ icon = get_icon_path("wide-angle"), icon_size = DIR.icon_size, scale = 0.25, shift = {-8,-8} },
}
photon_turret.collision_box = standard_3x3_collision()
photon_turret.selection_box = standard_3x3_selection()
photon_turret.tile_width = 3
photon_turret.tile_height = 3
photon_turret.corpse = "big-remnants"
photon_turret.dying_explosion = "medium-explosion"
photon_turret.folding_speed = 0.04
photon_turret.preparing_speed = 0.04
photon_turret.rotation_speed = 0.005
photon_turret.max_health = 2000
photon_turret.turret_base_has_direction = true
photon_turret.prepare_range = 65
photon_turret.entity_info_icon_shift = standard_3x3_turret_info_icon_shift()

table.insert(photon_turret.flags, "building-direction-8-way")

photon_turret.energy_source = {
    buffer_capacity = DIR.photon.buffer .. "MJ",
    drain = "120kW",
    input_flow_limit = (DIR.photon.torpedo_cost / DIR.photon.charge_time) .. "MW",
    type = "electric",
    usage_priority = "primary-input"
}

photon_turret.attack_parameters = {
    ammo_type = {
        action = {
            action_delivery = {
                projectile = "photon-torpedo",
                type = "projectile",
                starting_speed = 0.5,
                max_range = 60,
                min_range = 12,
                direction_deviation = 0,
                source_effects = {
                    entity_name = "photon-muzzle",
                    type = "create-explosion"
                },
            },
            type = "direct",
        },
        category = "photon-torpedo",
        energy_consumption = DIR.photon.torpedo_cost .. "MJ",
        target_type = "entity",
    },
    cooldown = DIR.photon.charge_time * 60,
    range = 60,
    min_range = 12,
    turn_range = 1/3,
    projectile_center = {0,-0.2},
    projectile_creation_distance = 1.8,
    lead_target_for_projectile_speed = 0.5,
    sound = {
		filename = string.format("%s/%s.ogg", DIR.sound_path, "new-photon"),
		volume = 1
    },
    type = "projectile"
}

photon_turret.base_picture = {
    layers = {
        get_layer("turrets/large-turret-base", 1, 1, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,0}),
        get_layer("turrets/large-turret-base-mask", 1, 1, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,0}, nil, {"mask"}, nil, nil, true),
    },
}
photon_turret.attacking_animation = {
    layers = {
        get_layer("turrets/photon-turret-shooting-shadow", 1, 8, true, nil, nil, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 128),
        get_layer("turrets/photon-turret-shooting", 1, 16, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 128),
        get_layer("turrets/photon-turret-shooting-mask", 1, 16, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 128, true),
    }
}
photon_turret.prepared_animation = {
    layers = {
        get_layer("turrets/photon-turret-shooting-shadow", 1, 8, true, nil, nil, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 128),
        get_layer("turrets/photon-turret-shooting", 1, 16, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 128),
        get_layer("turrets/photon-turret-shooting-mask", 1, 16, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 128, true),
    }
}
photon_turret.folded_animation = {
    layers = {
        get_layer("turrets/photon-turret-raising-shadow", 1, 1, true, nil, nil, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 8),
        get_layer("turrets/photon-turret-raising", 1, 1, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 8),
        get_layer("turrets/photon-turret-raising-mask", 1, 1, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 8, true),
    }
}
photon_turret.folding_animation = {
    layers = {
        get_layer("turrets/photon-turret-raising-shadow", 1, 1, true, 4, nil, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 8, nil),
        get_layer("turrets/photon-turret-raising", 4, 4, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 8, nil, "backward"),
        get_layer("turrets/photon-turret-raising-mask", 4, 4, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 8, true, "backward"),
    }
}
photon_turret.preparing_animation = {
    layers = {
        get_layer("turrets/photon-turret-raising-shadow", 1, 1, true, 4, nil, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 8, nil),
        get_layer("turrets/photon-turret-raising", 4, 4, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 8),
        get_layer("turrets/photon-turret-raising-mask", 4, 4, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 8, true),
    }
}
photon_turret.energy_glow_animation = nil

data:extend({photon_turret})

local narrow_photon = table.deepcopy(photon_turret)
narrow_photon.attack_parameters.turn_range = 1/6
narrow_photon.name = narrow_photon.name .. "-narrow"
narrow_photon.energy_source.drain = "60kW"
narrow_photon.icons[2].icon = get_icon_path("narrow-angle")

data:extend({narrow_photon})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- minigun turret

local minigun = table.deepcopy(gun_turret)
minigun.name = "minigun-turret"
minigun.icon = get_icon_path("minigun-turret")
minigun.icon_size = DIR.icon_size
minigun.minable.result = "minigun-turret"
minigun.collision_box = standard_3x3_collision()
minigun.selection_box = standard_3x3_selection()
minigun.tile_width = 3
minigun.tile_height = 3
minigun.corpse = "big-remnants"
minigun.dying_explosion = "medium-explosion"
minigun.rotation_speed = 0.005
minigun.attack_parameters.ammo_category = "belt"
minigun.attack_parameters.range = DIR.ammo.range.belt
minigun.attack_parameters.cooldown = DIR.ammo.turret_speed.belt
minigun.attack_parameters.projectile_center = {0,-0.2}
minigun.attack_parameters.projectile_creation_distance = 1.9
minigun.attack_parameters.lead_target_for_projectile_speed = DIR.projectile_speed.bullet
minigun.max_health = 1500
minigun.prepare_range = DIR.ammo.range.belt + 5
minigun.attacking_speed = 1/3
minigun.attack_parameters.shell_particle.creation_distance = -2.5
minigun.entity_info_icon_shift = standard_3x3_turret_info_icon_shift()

minigun.base_picture = {
    layers = {
        get_layer("turrets/large-turret-base", 1, 1, false, nil, 0.5, 192, 192, 0, 0, 192, 192, {0,0}),
        get_layer("turrets/large-turret-base-mask", 1, 1, false, nil, 0.5, 192, 192, 0, 0, 192, 192, {0,0}, nil, {"mask"}, nil, nil, true),
    },
}
minigun.attacking_animation = {
    layers = {
        get_multi_layer("turrets/minigun-turret-shooting-shadow", 8, 3, nil, true, nil, 0.5, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 128),
        get_multi_layer("turrets/minigun-turret-shooting", 8, 1, nil, false, 3, 0.5, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 128),
        get_multi_layer("turrets/minigun-turret-shooting-mask", 8, 1, nil, false, 3, 0.5, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 128, true),
        get_multi_layer("turrets/minigun-turret-shooting-barrel", 8, 3, nil, false, nil, 0.5, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 128),
        get_multi_layer("turrets/minigun-turret-shooting-bullets", 8, 3, nil, false, nil, 0.5, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 128),
    }
}
minigun.prepared_animation = {
    layers = {
        get_multi_layer("turrets/minigun-turret-shooting-shadow", 8, 1, nil, true, nil, nil, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 128),
        get_multi_layer("turrets/minigun-turret-shooting", 8, 1, nil, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 128),
        get_multi_layer("turrets/minigun-turret-shooting-mask", 8, 1, nil, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 128, true),
        get_multi_layer("turrets/minigun-turret-shooting-barrel", 8, 1, nil, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 128),
        get_multi_layer("turrets/minigun-turret-shooting-bullets", 8, 1, nil, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 128),
    }
}
minigun.preparing_animation = {
    layers = {
        get_layer("turrets/minigun-turret-raising-shadow", 8, 0, true, nil, nil, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 4),
        get_layer("turrets/minigun-turret-raising", 8, 0, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 4),
        get_layer("turrets/minigun-turret-raising-mask", 8, 0, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 4, true),
    }
}
minigun.folding_animation = {
    layers = {
        get_layer("turrets/minigun-turret-raising-shadow", 8, 0, true, nil, nil, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 4, false, "backward"),
        get_layer("turrets/minigun-turret-raising", 8, 0, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 4, nil, "backward"),
        get_layer("turrets/minigun-turret-raising-mask", 8, 0, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 4, true, "backward"),
    }
}
minigun.folded_animation = {
    layers = {
        get_layer("turrets/minigun-turret-raising-shadow", 1, 0, true, nil, nil, 320, 192, 0, 0, 320, 192, {1,0}, nil, nil, nil, 4),
        get_layer("turrets/minigun-turret-raising", 1, 0, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, nil, nil, 4),
        get_layer("turrets/minigun-turret-raising-mask", 1, 0, false, nil, nil, 192, 192, 0, 0, 192, 192, {0,-1}, nil, {"mask"}, nil, 4, true),
    }
}

data:extend({minigun})

------------------------------------------------------------------------------------------------------------------------------------------------------
