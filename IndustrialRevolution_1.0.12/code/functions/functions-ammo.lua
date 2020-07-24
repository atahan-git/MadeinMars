------------------------------------------------------------------------------------------------------------------------------------------------------

function get_projectile_name(base_damage, hue, saturation)
	if hue == nil then hue = 0 end
	return "bullet-"..math.floor(hue*1000).."-"..math.floor(get_saturation(hue,saturation)*100).."-"..base_damage
end

function create_projectile(base_damage, hue, saturation)
	local name = get_projectile_name(base_damage, hue, saturation)
	if not data.raw.projectile[name] then
		data:extend({{
			name = name,
			type = "projectile",
			acceleration = 0,
			action = {
				action_delivery = {
					target_effects = {
						damage = {
							amount = base_damage,
							type = "physical"
						},
						type = "damage"
					},
					type = "instant"
				},
				type = "direct"
			},
			animation = {
				filename = DIR.projectiles_path.."/bullet.png",
				tint = hsva2rgba(hue,get_saturation(hue,saturation)/4,1,1),
				frame_count = 1,
				height = 100,
				width = 7,
				scale = 0.5,
				priority = "high",
			},
			collision_box = {{-0.05,-0.25},{0.05,0.25}},
			direction_only = true,
			flags = {"not-on-map"},
			--hit_at_collision_position = true,
			force_condition = "not-same",
		}})
	end
end

function get_bullet_ammo_type(base_damage, hue, saturation)
    return {
        action = {
            {
                action_delivery = {
                    source_effects = {
                        {
                            entity_name = "explosion-gunshot",
                            type = "create-explosion"
                        }
                    },
                    type = "instant"
                },
                type = "direct"
            },
            {
                action_delivery = {
                    direction_deviation = 0.05,
                    max_range = DIR.ammo.range.magazine,
                    projectile = get_projectile_name(base_damage, hue, saturation),
                    range_deviation = 0,
                    starting_speed = DIR.projectile_speed.bullet,
                    type = "projectile"
                },
                repeat_count = 1,
                type = "direct"
            }
        },
        category = "bullet",
        target_type = "entity"
    }
end

function get_cartridge_ammo_type(base_damage, pellets, hue, saturation)
    return {
        action = {
            {
                action_delivery = {
                    source_effects = {
                        {
                            entity_name = "explosion-gunshot",
                            type = "create-explosion"
                        }
                    },
                    type = "instant"
                },
                type = "direct"
            },
            {
                action_delivery = {
                    direction_deviation = 0.3,
                    max_range = DIR.ammo.range.cartridge,
                    projectile = get_projectile_name(base_damage, hue, saturation),
                    range_deviation = 0.3,
                    starting_speed = DIR.projectile_speed.shotgun,
                    type = "projectile"
                },
                repeat_count = pellets,
                type = "direct"
            }
        },
        category = "shotgun-shell",
        target_type = "direction"
    }
end

function get_belt_ammo_type(base_damage, hue, saturation)
    return {
        action = {
            {
                action_delivery = {
                    source_effects = {
                        {
                            entity_name = "explosion-gunshot",
                            type = "create-explosion"
                        }
                    },
                    type = "instant"
                },
                type = "direct"
            },
            {
                action_delivery = {
                    direction_deviation = 0.075,
                    max_range = DIR.ammo.range.belt,
                    projectile = get_projectile_name(base_damage, hue, saturation),
                    range_deviation = 0,
                    starting_speed = DIR.projectile_speed.bullet,
                    type = "projectile",
                },
                repeat_count = 1,
                type = "direct"
            }
        },
        category = "belt",
        target_type = "entity"
    }
end


------------------------------------------------------------------------------------------------------------------------------------------------------
