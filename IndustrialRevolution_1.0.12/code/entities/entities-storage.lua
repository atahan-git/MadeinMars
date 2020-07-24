------------------------------------------------------------------------------------------------------------------------------------------------------

-- STORAGE DEPOTS

------------------------------------------------------------------------------------------------------------------------------------------------------

-- steel storage depot

local circuits = circuit_connector_definitions.create (
	universal_connector_template,
	{
		{ variation = 26, main_offset = util.by_pixel(9.5,32.5), shadow_offset = util.by_pixel(9.5,32.5), show_shadow = true }
	}
)

local storage_depot = {
	name = "steel-storage-depot",
	type = "container",
	circuit_connector_sprites = circuits.sprites,
	circuit_wire_connection_point = circuits.points,
	circuit_wire_max_distance = 9,
	collision_box = standard_3x3_collision(),
	selection_box = standard_3x3_selection(),
	corpse = "big-remnants",
	fast_replaceable_group = "storage-depot",
	flags = {"placeable-neutral","player-creation"},
	icon = get_icon_path("steel-storage-depot", DIR.icon_size),
	icon_size = DIR.icon_size,
	inventory_size = 800,
	max_health = 1000,
	minable = {
		mining_time = 0.5,
		result = "steel-storage-depot"
	},
	open_sound = {
		filename = "__base__/sound/metallic-chest-open.ogg",
        volume = 0.65
	},
	close_sound = {
		filename = "__base__/sound/metallic-chest-close.ogg",
        volume = 0.65
	},
	picture = {
		layers = {
			get_layer("storage/steel-storage-depot-base", nil, nil, false, nil, 1, 192, 192, 0, 0, 192, 192, {0,0}),
			get_layer("storage/depot-shadow", nil, nil, true, nil, 1, 320, 256, 0, 0, 320, 256, {1.0,0}),		
		}
	},
	vehicle_impact_sound = {
		filename = "__base__/sound/car-metal-impact.ogg",
		volume = 1
	}
}
data:extend({table.deepcopy(storage_depot)})

-- bronze version

local bronze = table.deepcopy(storage_depot)
bronze.name = "bronze-storage-depot"
bronze.inventory_size = 400
bronze.max_health = 750
bronze.icon = get_icon_path("bronze-storage-depot", DIR.icon_size)
bronze.minable.result = "bronze-storage-depot"
bronze.picture = {
	layers = {
		get_layer("storage/bronze-storage-depot-base", nil, nil, false, nil, 1, 192, 192, 0, 0, 192, 192, {0,0}),
		get_layer("storage/depot-shadow", nil, nil, true, nil, 1, 320, 256, 0, 0, 320, 256, {1.0,0}),		
	}
}
bronze.next_upgrade = "steel-storage-depot"
data:extend({table.deepcopy(bronze)})

-- logistics versions

local logistic_depot = table.deepcopy(storage_depot)
logistic_depot.type = "logistic-container"
logistic_depot.opened_duration = 8
logistic_depot.icons = { { icon = get_icon_path("logistics-depot-base", DIR.icon_size), icon_size = DIR.icon_size } }
logistic_depot.icon = nil
logistic_depot.animation = {
	layers = {
		get_layer("storage/depot-shadow", nil, nil, true, 8, 1, 320, 256, 0, 0, 320, 256, {1.0,0}),		
		get_layer("storage/logistics-depot-base", 8, 4, false, nil, 1, 192, 192, 0, 0, 192, 192, {0,0}),
	}
}
logistic_depot.picture = nil

local instance = table.deepcopy(logistic_depot)
instance.name = "logistics-depot-active"
instance.minable.result = "logistics-depot-active"
instance.logistic_mode = "active-provider"
table.insert(instance.animation.layers, get_layer("storage/logistics-depot-mask", nil, nil, false, 8, 1, 192, 192, 0, 0, 192, 192, {0,0}, nil, nil, DIR.icon_masks["logistics-depot-active"].tint))
table.insert(instance.icons, { icon = get_icon_path("logistics-depot-mask", DIR.icon_size), icon_size = DIR.icon_size, tint = DIR.icon_masks["logistics-depot-active"].tint } )
data:extend({table.deepcopy(instance)})

local instance = table.deepcopy(logistic_depot)
instance.name = "logistics-depot-passive"
instance.minable.result = "logistics-depot-passive"
instance.logistic_mode = "passive-provider"
table.insert(instance.animation.layers, get_layer("storage/logistics-depot-mask", nil, nil, false, 8, 1, 192, 192, 0, 0, 192, 192, {0,0}, nil, nil, DIR.icon_masks["logistics-depot-passive"].tint))
table.insert(instance.icons, { icon = get_icon_path("logistics-depot-mask", DIR.icon_size), icon_size = DIR.icon_size, tint = DIR.icon_masks["logistics-depot-passive"].tint } )
data:extend({table.deepcopy(instance)})

local instance = table.deepcopy(logistic_depot)
instance.name = "logistics-depot-storage"
instance.minable.result = "logistics-depot-storage"
instance.logistic_slots_count = 1
instance.logistic_mode = "storage"
table.insert(instance.animation.layers, get_layer("storage/logistics-depot-mask", nil, nil, false, 8, 1, 192, 192, 0, 0, 192, 192, {0,0}, nil, nil, DIR.icon_masks["logistics-depot-storage"].tint))
table.insert(instance.icons, { icon = get_icon_path("logistics-depot-mask", DIR.icon_size), icon_size = DIR.icon_size, tint = DIR.icon_masks["logistics-depot-storage"].tint } )
data:extend({table.deepcopy(instance)})

local instance = table.deepcopy(logistic_depot)
instance.name = "logistics-depot-requester"
instance.minable.result = "logistics-depot-requester"
instance.logistic_mode = "requester"
instance.logistic_slots_count = 24
table.insert(instance.animation.layers, get_layer("storage/logistics-depot-mask", nil, nil, false, 8, 1, 192, 192, 0, 0, 192, 192, {0,0}, nil, nil, DIR.icon_masks["logistics-depot-requester"].tint))
table.insert(instance.icons, { icon = get_icon_path("logistics-depot-mask", DIR.icon_size), icon_size = DIR.icon_size, tint = DIR.icon_masks["logistics-depot-requester"].tint } )
data:extend({table.deepcopy(instance)})

local instance = table.deepcopy(logistic_depot)
instance.name = "logistics-depot-buffer"
instance.minable.result = "logistics-depot-buffer"
instance.logistic_mode = "buffer"
instance.logistic_slots_count = 24
table.insert(instance.animation.layers, get_layer("storage/logistics-depot-mask", nil, nil, false, 8, 1, 192, 192, 0, 0, 192, 192, {0,0}, nil, nil, DIR.icon_masks["logistics-depot-buffer"].tint))
table.insert(instance.icons, { icon = get_icon_path("logistics-depot-mask", DIR.icon_size), icon_size = DIR.icon_size, tint = DIR.icon_masks["logistics-depot-active"].tint } )
data:extend({table.deepcopy(instance)})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- bronze chest

local bronze_chest = {
	name = "bronze-chest",
	type = "container",
	circuit_connector_sprites = data.raw.container["iron-chest"].circuit_connector_sprites,
	circuit_wire_connection_point = data.raw.container["iron-chest"].circuit_wire_connection_point,
	circuit_wire_max_distance = 9,
	collision_box = standard_small_chest_collision(),
	selection_box = {{-0.5,-0.5}, {0.5,0.5}},
	corpse = "small-remnants",
	fast_replaceable_group = "container",
	flags = {"placeable-neutral","player-creation"},
	icon = get_icon_path("bronze-chest", DIR.icon_size),
	icon_size = DIR.icon_size,
	inventory_size = 40,
	max_health = 175,
	minable = {
		mining_time = 0.5,
		result = "bronze-chest"
	},
	next_upgrade = "iron-chest",
	open_sound = {
		filename = "__base__/sound/metallic-chest-open.ogg",
        volume = 0.65
	},
	close_sound = {
		filename = "__base__/sound/metallic-chest-close.ogg",
        volume = 0.65
	},
	picture = {
		layers = {
			get_layer("storage/bronze-chest-base", nil, nil, false, nil, 1, 96, 96, 0, 0, 96, 96, {0,0}),
			get_layer("storage/chest-shadow", nil, nil, true, nil, 1, 128, 96, 0, 0, 128, 96, {0.25,0}),		
		}
	},
	vehicle_impact_sound = {
		filename = "__base__/sound/car-metal-impact.ogg",
		volume = 1
	}
}
data:extend({bronze_chest})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- tin chest

local tin_chest = {
	name = "tin-chest",
	type = "container",
	circuit_connector_sprites = data.raw.container["iron-chest"].circuit_connector_sprites,
	circuit_wire_connection_point = data.raw.container["iron-chest"].circuit_wire_connection_point,
	circuit_wire_max_distance = 9,
	collision_box = standard_small_chest_collision(),
	selection_box = {{-0.5,-0.5}, {0.5,0.5}},
	corpse = "small-remnants",
	fast_replaceable_group = "container",
	flags = {"placeable-neutral","player-creation"},
	icon = get_icon_path("tin-chest", DIR.icon_size),
	icon_size = DIR.icon_size,
	inventory_size = 20,
	max_health = 100,
	minable = {
		mining_time = 0.5,
		result = "tin-chest"
	},
	next_upgrade = "bronze-chest",
	open_sound = {
		filename = "__base__/sound/metallic-chest-open.ogg",
        volume = 0.65
	},
	close_sound = {
		filename = "__base__/sound/metallic-chest-close.ogg",
        volume = 0.65
	},
	picture = {
		layers = {
			get_layer("storage/tin-chest-base", nil, nil, false, nil, 1, 96, 96, 0, 0, 96, 96, {0,0}),
			get_layer("storage/chest-shadow", nil, nil, true, nil, 1, 128, 96, 0, 0, 128, 96, {0.25,0}),		
		}
	},
	vehicle_impact_sound = {
		filename = "__base__/sound/car-metal-impact.ogg",
		volume = 1
	}
}
data:extend({tin_chest})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- gold chest (oooh)

local gold_chest = {
	name = "gold-chest",
	type = "container",
	circuit_connector_sprites = data.raw.container["iron-chest"].circuit_connector_sprites,
	circuit_wire_connection_point = data.raw.container["iron-chest"].circuit_wire_connection_point,
	circuit_wire_max_distance = 9,
	collision_box = standard_small_chest_collision(),
	selection_box = {{-0.5,-0.5}, {0.5,0.5}},
	corpse = "small-remnants",
	fast_replaceable_group = "container",
	flags = {"placeable-neutral","player-creation"},
	icon = get_icon_path("gold-chest", DIR.icon_size),
	icon_size = DIR.icon_size,
	inventory_size = 60,
	max_health = 125,
	minable = {
		mining_time = 0.5,
		result = "gold-chest"
	},
	open_sound = {
		filename = "__base__/sound/metallic-chest-open.ogg",
        volume = 0.65
	},
	close_sound = {
		filename = "__base__/sound/metallic-chest-close.ogg",
        volume = 0.65
	},
	picture = {
		layers = {
			get_layer("storage/gold-chest-base", nil, nil, false, nil, 1, 96, 96, 0, 0, 96, 96, {0,0}),
			get_layer("storage/chest-shadow", nil, nil, true, nil, 1, 128, 96, 0, 0, 128, 96, {0.25,0}),		
		}
	},
	vehicle_impact_sound = {
		filename = "__base__/sound/car-metal-impact.ogg",
		volume = 1
	}
}
data:extend({gold_chest})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- vanilla small chests reskin

local chest = data.raw.container["iron-chest"]

chest.collision_box = standard_small_chest_collision()
chest.selection_box = {{-0.5,-0.5}, {0.5,0.5}}
chest.icon = get_icon_path("iron-chest", DIR.icon_size)
chest.icon_size = DIR.icon_size
chest.picture = {
	layers = {
		get_layer("storage/iron-chest-base", nil, nil, false, nil, 1, 96, 96, 0, 0, 96, 96, {0,0}),
		get_layer("storage/chest-shadow", nil, nil, true, nil, 1, 128, 96, 0, 0, 128, 96, {0.25,0}),		
	}
}

chest = data.raw.container["steel-chest"]

chest.collision_box = standard_small_chest_collision()
chest.selection_box = {{-0.5,-0.5}, {0.5,0.5}}
chest.icon = get_icon_path("steel-chest", DIR.icon_size)
chest.icon_size = DIR.icon_size
chest.picture = {
	layers = {
		get_layer("storage/steel-chest-base", nil, nil, false, nil, 1, 96, 96, 0, 0, 96, 96, {0,0}),
		get_layer("storage/chest-shadow", nil, nil, true, nil, 1, 128, 96, 0, 0, 128, 96, {0.25,0}),		
	}
}


------------------------------------------------------------------------------------------------------------------------------------------------------
