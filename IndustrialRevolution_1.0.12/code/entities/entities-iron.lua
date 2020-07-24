------------------------------------------------------------------------------------------------------------------------------------------------------

-- IRON MACHINES

------------------------------------------------------------------------------------------------------------------------------------------------------

-- iron template

local speed = 1
local animspeed = DIR.anim_speed / speed
local energy = get_scaled_energy_usage(speed, 0.04)
local iron = {
	crafting_speed = speed,
	energy_usage = energy.active,
	energy_source = {
		type = "electric",
		usage_priority = "secondary-input",
		drain = energy.passive,
		emissions_per_minute = 1,
	},
	allowed_effects = {
		"consumption",
		"speed",
		"pollution",
		"productivity",
	},
	module_specification = {
		module_info_icon_shift = standard_entity_info_module_shift(),
		module_slots = 2
	},
	max_health = 400,
	dying_explosion = "medium-explosion",
	resistances = {
	  {
		type = "fire",
		percent = 80
	  },
	},
	corpse = "big-remnants",
	flags = {"placeable-player", "placeable-neutral", "player-creation"},
	collision_box = standard_3x3_collision() ,
	selection_box = standard_3x3_selection() ,
	working_sound = {
		sound = {
			filename = "__base__/sound/furnace.ogg",
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
		filename = "__core__/sound/deconstruct-large.ogg"
	},
	vehicle_impact_sound = {
		filename = "__base__/sound/car-metal-impact.ogg",
		volume = 0.65
	},
	tile_width = 3,
	tile_height = 3,
	entity_info_icon_shift = standard_entity_info_icon_shift(),
}

local iron_light = { color = DIR.indicator_colours.iron, intensity = 0.5, size = 9, shift = {0,0.5} }
local glow = get_layer("machines/assembler-glow", 30, 5, false, nil, animspeed, 192, 128, 0, 0, 192, 128, {0,0}, "additive", nil, {r=0.5,g=1.0,b=1.0})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- iron grinder

local iron_grinder = table.deepcopy(iron)
iron_grinder.name = "iron-grinder"
iron_grinder.type = "assembling-machine"
iron_grinder.icon = get_icon_path("iron-grinder", DIR.icon_size)
iron_grinder.icon_size = DIR.icon_size
iron_grinder.crafting_categories = {"grinding","grinding-1","grinding-2"}
iron_grinder.allowed_effects = {
	"consumption",
	"speed",
	"productivity",
	"pollution",
}
iron_grinder.gui_title_key = "gui-title.grinding"
iron_grinder.animation = {
	layers = {
		get_layer("machines/iron-grinder-base", 30, 5, false, nil, animspeed, 192, 192, 0, 0, 192, 192, {0,0}),
		get_layer("machines/iron-shadow", nil, nil, true, 30, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
	}
}
iron_grinder.working_visualisations = {
	{
		animation = glow,
		light = iron_light,
	},
}
iron_grinder.placeable_by = {item = "iron-grinder", count = 1}
iron_grinder.minable = {
	mining_time = 0.5,
	result = "iron-grinder"
}
iron_grinder.fast_replaceable_group = "grinder"
iron_grinder.next_upgrade = "steel-grinder"
iron_grinder.working_sound.sound.filename = string.format("%s/%s.ogg", DIR.sound_path, "grinder")
iron_grinder.energy_source.emissions_per_minute = 1.5
iron_grinder.entity_info_icon_shift = standard_grinder_info_icon_shift()
data:extend({iron_grinder})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- iron scrapper

local iron_scrapper = table.deepcopy(iron)
iron_scrapper.name = "iron-scrapper"
iron_scrapper.type = "furnace"
iron_scrapper.source_inventory_size = 1
iron_scrapper.result_inventory_size = 5
iron_scrapper.icon = get_icon_path("iron-scrapper", DIR.icon_size)
iron_scrapper.icon_size = DIR.icon_size
iron_scrapper.crafting_categories = {"scrapping"}
iron_scrapper.crafting_speed = 1
iron_scrapper.energy_source.emissions_per_minute = 18
iron_scrapper.match_animation_speed_to_activity = false
iron_scrapper.allowed_effects = {
	"consumption",
	"speed",
	"pollution",
}
iron_scrapper.animation = {
	layers = {
		get_layer("machines/iron-scrapper-base", 30, 5, false, nil, DIR.anim_speed, 192, 192, 0, 0, 192, 192, {0,0}),
		get_layer("machines/iron-shadow", nil, nil, true, 30, DIR.anim_speed, 256, 192, 0, 0, 256, 192, {0.5,0}),
	}
}
iron_scrapper.working_visualisations = {
	{
		animation = get_layer("machines/assembler-glow", 30, 5, false, nil, DIR.anim_speed, 192, 128, 0, 0, 192, 128, {0,0}, "additive", nil, {r=0.5,g=1.0,b=1.0}),
		light = iron_light,
	},
}
iron_scrapper.placeable_by = {item = "iron-scrapper", count = 1}
iron_scrapper.minable = {
	mining_time = 0.5,
	result = "iron-scrapper"
}
iron_scrapper.fast_replaceable_group = "scrapper"
iron_scrapper.working_sound.sound.filename = string.format("%s/%s.ogg", DIR.sound_path, "grinder")
iron_scrapper.entity_info_icon_shift = standard_grinder_info_icon_shift()
data:extend({iron_scrapper})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- iron furnace

speed = 2
energy = get_scaled_energy_usage(speed, 0.04)

local iron_furnace = table.deepcopy(iron)
iron_furnace.name = "iron-furnace"
iron_furnace.type = "assembling-machine"
iron_furnace.crafting_speed = speed
iron_furnace.energy_source = {
	type = "electric",
	usage_priority = "secondary-input",
	drain = energy.passive,
	emissions_per_minute = speed * 2, -- iron furnace deliberately lower
}
iron_furnace.energy_usage = energy.active
iron_furnace.icon = get_icon_path("iron-furnace", DIR.icon_size)
iron_furnace.icon_size = DIR.icon_size
iron_furnace.crafting_categories = {"smelting","smelting-1","smelting-2","smelting-3"}
iron_furnace.allowed_effects = {
	"consumption",
	"speed",
	"productivity",
	"pollution",
}
iron_furnace.gui_title_key = "gui-title.smelting"
iron_furnace.animation = data.raw["furnace"]["electric-furnace"].animation
iron_furnace.working_visualisations = data.raw["furnace"]["electric-furnace"].working_visualisations
iron_furnace.placeable_by = {item = "iron-furnace", count = 1}
iron_furnace.minable = {
	mining_time = 0.5,
	result = "iron-furnace"
}
iron_furnace.fast_replaceable_group = "furnace"
iron_furnace.working_sound.sound.filename = string.format("%s/%s.ogg", DIR.base_sound_path, "electric-furnace")
data:extend({iron_furnace})

------------------------------------------------------------------------------------------------------------------------------------------------------

