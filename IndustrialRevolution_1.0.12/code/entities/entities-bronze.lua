------------------------------------------------------------------------------------------------------------------------------------------------------

-- BRONZE MACHINES

------------------------------------------------------------------------------------------------------------------------------------------------------

-- bronze template

local speed = 1
local animspeed = DIR.anim_speed / speed
local energy = get_scaled_energy_usage(speed, 0)
local bronze = {
	crafting_speed = speed,
	energy_source = {
		effectivity = 1,
		emissions_per_minute = 3,
		fuel_category = "chemical",
		fuel_inventory_size = 1,
		type = "burner"
	},
	energy_usage = energy.active,
	allowed_effects = {
		"pollution"
	},
	module_specification = {
		module_info_icon_shift = standard_entity_info_module_shift(),
		module_slots = 1
	},
	max_health = 250,
	dying_explosion = "medium-explosion",
	resistances = {
	  {
		type = "fire",
		percent = 90
	  },
	},
	corpse = "big-remnants",
	flags = {"placeable-player", "placeable-neutral", "player-creation"},
	collision_box = standard_3x3_collision(),
	selection_box = standard_3x3_selection(),
	working_sound = {
		sound = {
			filename = "__base__/sound/furnace.ogg",
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

------------------------------------------------------------------------------------------------------------------------------------------------------

-- bronze grinder

local bronze_grinder = table.deepcopy(bronze)
bronze_grinder.name = "bronze-grinder"
bronze_grinder.type = "assembling-machine"
bronze_grinder.result_inventory_size = 1
bronze_grinder.source_inventory_size = 1
bronze_grinder.icon = get_icon_path("bronze-grinder", DIR.icon_size)
bronze_grinder.icon_size = DIR.icon_size
bronze_grinder.crafting_categories = {"grinding","grinding-1"}
bronze_grinder.gui_title_key = "gui-title.grinding"
bronze_grinder.animation = {
	layers = {
		get_layer("machines/bronze-grinder-base", 30, 5, false, nil, animspeed, 192, 192, 0, 0, 192, 192, {0,0}),
		get_layer("machines/copper-shadow", nil, nil, true, 30, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
	}
}
bronze_grinder.working_visualisations = {
	{
		animation = get_layer("machines/bronze-grinder-working", 30, 5, false, nil, animspeed, 64, 64, 0, 0, 64, 64, {0,1}, "additive"),
	}
}
bronze_grinder.energy_source.smoke = {
	{
		frequency = 5,
		name = "smoke",
		position = { 0, -0.5 },
		starting_frame_deviation = 60,
		starting_vertical_speed = 0.08
	},
}
bronze_grinder.placeable_by = {item = "bronze-grinder", count = 1}
bronze_grinder.fast_replaceable_group = "grinder"
bronze_grinder.next_upgrade = "iron-grinder"
bronze_grinder.minable = {
	mining_time = 0.5,
	result = "bronze-grinder"
}
bronze_grinder.working_sound.sound.filename = string.format("%s/%s.ogg", DIR.sound_path, "grinder")
bronze_grinder.entity_info_icon_shift = standard_grinder_info_icon_shift()
data:extend({table.deepcopy(bronze_grinder)})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- bronze furnace

speed = 1.5
animspeed = DIR.anim_speed / speed
energy = get_scaled_energy_usage(speed, 0)

local bronze_furnace = table.deepcopy(bronze)
bronze_furnace.name = "bronze-furnace"
bronze_furnace.crafting_speed = speed
bronze_furnace.energy_source = {
	effectivity = 1,
	emissions_per_minute = speed * 3,
	fuel_category = "chemical",
	fuel_inventory_size = 1,
	type = "burner"
}
bronze_furnace.energy_usage = energy.active
bronze_furnace.type = "assembling-machine"
bronze_furnace.icon = get_icon_path("bronze-furnace", DIR.icon_size)
bronze_furnace.icon_size = DIR.icon_size
bronze_furnace.crafting_categories = {"smelting","smelting-1","smelting-2"}
bronze_furnace.gui_title_key = "gui-title.smelting"
bronze_furnace.animation = {
	layers = {
		get_layer("machines/bronze-furnace-base", nil, nil, false, 30, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}),
		get_layer("machines/bronze-furnace-working", 30, 10, false, nil, animspeed, 32, 64, 0, 0, 32, 64, {0.25,-1.5}),
		get_layer("machines/bronze-furnace-shadow", nil, nil, true, 30, animspeed, 384, 192, 0, 0, 384, 192, {1.5,0}),
	}
}
bronze_furnace.working_visualisations = {
	{
		animation = get_layer("machines/bronze-furnace-glow", 30, 5, false, nil, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}, "additive"),
	}
}
bronze_furnace.energy_source.smoke = {
	{
		frequency = 5,
		name = "smoke",
		position = { -1.1, -1.75 },
		starting_frame_deviation = 60,
		starting_vertical_speed = 0.08
	},
	{
		frequency = 5,
		name = "smoke",
		position = { 1.1, -1.75 },
		starting_frame_deviation = 40,
		starting_vertical_speed = 0.08
	},
}
bronze_furnace.fast_replaceable_group = "furnace"
bronze_furnace.next_upgrade = "iron-furnace"
bronze_furnace.minable = {
	mining_time = 0.5,
	result = "bronze-furnace"
}
bronze_furnace.placeable_by = {item = "bronze-furnace", count = 1}
data:extend({table.deepcopy(bronze_furnace)})

------------------------------------------------------------------------------------------------------------------------------------------------------
