------------------------------------------------------------------------------------------------------------------------------------------------------

-- generate an array of bullet-style projectiles from the materials available

for material,materialdata in pairs(DIR.materials) do
    if materialdata.base_damage then create_projectile(materialdata.base_damage, materialdata.hue, materialdata.saturation) end
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- tweak laser beam

local laser = data.raw.beam["laser-beam"]
local colour_y = DIR.laser_colours[settings.startup["deadlock-industry-laser-colour"].value].index

laser.body[1].filename = DIR.projectiles_path.."/laser-body.png"
laser.body[1].y = colour_y * 12
laser.head.filename = DIR.projectiles_path.."/laser-body.png"
laser.head.y = colour_y * 12
laser.tail.filename = DIR.projectiles_path.."/laser-end.png"
laser.tail.y = colour_y * 62
laser.tail.shift = {23/64,0}
laser.light_animations.body[1].filename = DIR.projectiles_path.."/laser-body-light.png"
laser.light_animations.head.filename = DIR.projectiles_path.."/laser-body-light.png"
laser.light_animations.tail.filename = DIR.projectiles_path.."/laser-end-light.png"
laser.ground_light_animations.body.tint = DIR.laser_colours[settings.startup["deadlock-industry-laser-colour"].value].rgb
laser.ground_light_animations.head.tint = DIR.laser_colours[settings.startup["deadlock-industry-laser-colour"].value].rgb
laser.ground_light_animations.tail.tint = DIR.laser_colours[settings.startup["deadlock-industry-laser-colour"].value].rgb
laser.working_sound = {
	sound = { filename = DIR.sound_path.."/new-phaser-loop.ogg" },
	max_sounds_per_type = 3,
	apparent_volume = 2,
}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- EMP beam

local emp_beam = table.deepcopy(data.raw.beam["electric-beam"])
emp_beam.name = "emp-beam"
emp_beam.working_sound = {
	filename = string.format("%s/%s.ogg", DIR.sound_path, "zap"),
	volume = 0.75
}

emp_beam.body = {}
emp_beam.body_light = {}
for i=1,8 do
	local lightning_row = {
		animation_speed = 0.5,
		blend_mode = "additive-soft",
		filename = DIR.projectiles_path.."/lightning-"..i..".png",
		frame_count = 8,
		height = 64,
		line_length = 8,
		width = 64,
		scale = 0.5,
	}
    table.insert(emp_beam.body,table.deepcopy(lightning_row))
	lightning_row.filename = DIR.projectiles_path.."/lightning-"..i.."-light.png"
    table.insert(emp_beam.body_light,table.deepcopy(lightning_row))
end

emp_beam.head = {
    animation_speed = 0.5,
    blend_mode = "additive-soft",
    filename = DIR.projectiles_path.."/lightning-head.png",
    frame_count = 8,
    height = 64,
    line_length = 8,
    width = 64,
    scale = 0.5,
}
emp_beam.head_light = table.deepcopy(emp_beam.head)
emp_beam.head_light.filename = DIR.projectiles_path.."/lightning-head-light.png"

emp_beam.tail = {
    animation_speed = 0.5,
    blend_mode = "additive-soft",
    filename = DIR.projectiles_path.."/lightning-tail.png",
    frame_count = 8,
    height = 64,
    line_length = 8,
    width = 64,
    scale = 0.5,
}
emp_beam.tail_light = table.deepcopy(emp_beam.head)
emp_beam.tail_light.filename = DIR.projectiles_path.."/lightning-tail-light.png"

emp_beam.start = nil
emp_beam.start_light = nil
emp_beam.ending = nil
emp_beam.ending_light = nil

emp_beam.action = {
    action_delivery = {
        target_effects = {
            {
                sticker = "emp-sticker",
                type = "create-sticker"
            }
        },
        type = "instant",
    },
    force = "not-same",
    radius = 5,
    type = "area"
}
data:extend({emp_beam})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- EMP sticker

data:extend({
    {
        name = "emp-sticker",
        type = "sticker",
        duration_in_ticks = 20,
        flags = {},
        target_movement_modifier = 0.1,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- "photon torpedo"

data:extend({{
    type = "ammo-category",
    name = "photon-torpedo",
	bonus_gui_order = "kc",
}})

data:extend({{
    name = "photon-torpedo",
    type = "projectile",
    acceleration = 0, 
    max_speed = 0.5,
    action = {
        action_delivery = {
            target_effects = {
                {
                    entity_name = "photon-hit",
                    type = "create-entity"
                },
                {
                    entity_name = "photon-shock",
                    type = "create-entity"
                },
                {
                    damage = {
                        amount = 180,
                        type = "laser"
                    },
                    type = "damage"
                },
                {
                    check_buildability = true,
                    entity_name = "small-scorchmark",
                    type = "create-entity"
                },
                {
                    action = {
                        action_delivery = {
                            target_effects = {
                                {
                                    damage = {
                                        amount = 240,
                                        type = "explosion"
                                    },
                                    type = "damage"
                                },
								{
									entity_name = "explosion",
									type = "create-entity"
								}
                            },
                            type = "instant"
                        },
                        radius = 4,
                        type = "area"
                    },
                    type = "nested-result"
                }
            },
            type = "instant"
        },
        type = "direct",
    },
    animation = {
        filename = DIR.projectiles_path.."/photon.png",
        blend_mode = "additive",
        frame_count = 16,
        line_length = 4,
        height = 128,
        priority = "high",
        width = 128,
        scale = 0.5,
    },
    light = {
        intensity = 0.75,
        size = 8,
        color = {1,0.25,0,1},
    },
    collision_box = {{-0.5,-0.5},{0.5,0.5}},
    direction_only = true,
    flags = {"not-on-map"},
    force_condition = "not-same",
}})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- explosions

data:extend({
    {
        name = "photon-hit",
        animations = {
            {
                animation_speed = 1,
                filename = DIR.projectiles_path .. "/photon-hit.png",
                blend_mode = "additive",
                frame_count = 64,
                height = 128,
                line_length = 8,
                priority = "high",
                width = 128,
                scale = 0.5,
            }
        },
        flags = {
            "not-on-map"
        },
        light = {
            color = {
                b = 0.25,
                g = 0.5,
                r = 1
            },
            intensity = 1,
            size = 10
        },
        sound = {
            aggregation = {
                max_count = 1,
                remove = true
            },
            variations = {
                {
                    filename = "__base__/sound/fight/large-explosion-1.ogg",
                    volume = 0.8
                },
                {
                    filename = "__base__/sound/fight/large-explosion-2.ogg",
                    volume = 0.8
                }
            }
        },
        type = "explosion"
    },
    {
        name = "photon-shock",
        animations = {
            {
                animation_speed = 1,
                filename = DIR.projectiles_path .. "/photon-shock.png",
                blend_mode = "additive-soft",
                frame_count = 8,
                height = 256,
                line_length = 8,
                priority = "high",
                width = 256,
                scale = 1,
            }
        },
        flags = {
            "not-on-map"
        },
        type = "explosion"
    },
    {
        name = "photon-muzzle",
        animations = {
            {
                animation_speed = 0.5,
                filename = DIR.projectiles_path .. "/photon-muzzle.png",
                blend_mode = "additive-soft",
                frame_count = 8,
                height = 64,
                line_length = 8,
                priority = "high",
                width = 64,
                scale = 0.5,
            }
        },
        flags = {
            "not-on-map"
        },
        rotate = true,
        type = "explosion"
    },
})

------------------------------------------------------------------------------------------------------------------------------------------------------

