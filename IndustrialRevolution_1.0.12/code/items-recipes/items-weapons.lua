------------------------------------------------------------------------------------------------------------------------------------------------------

-- minigun

data:extend({
    {
        attack_parameters = {
            ammo_category = "belt",
            cooldown = 2,
            movement_slow_down_factor = 0.95,
            movement_slow_down_cooldown = seconds_to_ticks(1),
            projectile_creation_distance = 1.125,
            range = DIR.ammo.range.belt,
            shell_particle = {
                center = {
                    0,
                    0.1
                },
                creation_distance = -0.5,
                direction_deviation = 0.1,
                name = "shell-particle",
                speed = 0.1,
                speed_deviation = 0.03,
                starting_frame_speed = 0.4,
                starting_frame_speed_deviation = 0.1
            },
            sound = {
                {
                    filename = "__base__/sound/fight/light-gunshot-1.ogg",
                    volume = 0.3
                },
                {
                    filename = "__base__/sound/fight/light-gunshot-2.ogg",
                    volume = 0.3
                },
                {
                    filename = "__base__/sound/fight/light-gunshot-3.ogg",
                    volume = 0.3
                }
            },
            type = "projectile",
        },
        icon = get_icon_path("minigun"),
        icon_size = DIR.icon_size,
        name = "minigun",
        order = "a[basic-clips]-c[minigun]",
        stack_size = 5,
        subgroup = "gun",
        type = "gun",
    },
    {
        type = "recipe",
        name = "minigun",
        result = "minigun",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"steel-tube",8},
            {"steel-gear-wheel",4},
            {"titanium-rod",8},
            {"titanium-chassis-small",1},
        },
        energy_required = 15,
    },
	{
		type = "ammo-category",
		name = "belt",
		bonus_gui_order = "kd",
	},
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- nerf/buff/fix vehicle weapons

data.raw.gun["vehicle-machine-gun"].attack_parameters.cooldown = data.raw.gun["submachine-gun"].attack_parameters.cooldown
data.raw.gun["vehicle-machine-gun"].attack_parameters.range = data.raw.gun["submachine-gun"].attack_parameters.range

data.raw.gun["tank-machine-gun"].attack_parameters.cooldown = data.raw.gun["minigun"].attack_parameters.cooldown
data.raw.gun["tank-machine-gun"].attack_parameters.range = data.raw.gun["minigun"].attack_parameters.range
data.raw.gun["tank-machine-gun"].attack_parameters.ammo_category = data.raw.gun["minigun"].attack_parameters.ammo_category
data.raw.gun["tank-machine-gun"].localised_name = {"item-name.minigun"}
data.raw.gun["tank-machine-gun"].icon = data.raw.gun["minigun"].icon
data.raw.gun["tank-machine-gun"].icon_size = data.raw.gun["minigun"].icon_size

------------------------------------------------------------------------------------------------------------------------------------------------------
