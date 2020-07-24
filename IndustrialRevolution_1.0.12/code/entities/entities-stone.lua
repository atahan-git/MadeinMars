------------------------------------------------------------------------------------------------------------------------------------------------------

-- STONE MACHINES

------------------------------------------------------------------------------------------------------------------------------------------------------

-- stone template

local speed = 1
local animspeed = 1
local energy = get_scaled_energy_usage(speed, 0)
local stone = {
    crafting_speed = speed,
    energy_source = {
        effectivity = 1,
        emissions_per_minute = speed * 3,
        fuel_category = "chemical",
        fuel_inventory_size = 1,
        smoke = {
            {
                frequency = 10,
                name = "smoke",
                position = {
                    0,
                    -1.25
                },
                starting_frame_deviation = 60,
                starting_vertical_speed = 0.08
            },
        },
        type = "burner"
    },
    energy_usage = energy.active,
    allowed_effects = {
        "pollution"
    },
    max_health = 200,
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
        filename = "__base__/sound/deconstruct-bricks.ogg"
    },
    vehicle_impact_sound = {
        filename = "__base__/sound/car-stone-impact.ogg",
        volume = 0.65
    },
}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- stone furnace

local stone_furnace = table.deepcopy(stone)
stone_furnace.name = "stone-age-furnace"
stone_furnace.type = "assembling-machine"
stone_furnace.icon = get_icon_path("stone-age-furnace", DIR.icon_size)
stone_furnace.icon_size = DIR.icon_size
stone_furnace.crafting_categories = {"smelting","smelting-1"}
stone_furnace.gui_title_key = "gui-title.smelting"
stone_furnace.animation = {
    layers = {
        get_layer("machines/stone-furnace-base", nil, nil, false, nil, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}),
        get_layer("machines/stone-furnace-shadow", nil, nil, true, nil, animspeed, 320, 192, 0, 0, 320, 192, {1.0,0}),
    }
}
stone_furnace.working_visualisations = {
    {
        animation = get_layer("machines/stone-furnace-working", 30, 6, false, nil, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}, "additive"),
    }
}
stone_furnace.match_animation_speed_to_activity = false
stone_furnace.fast_replaceable_group = "furnace"
stone_furnace.next_upgrade = "bronze-furnace"
stone_furnace.minable = {
    mining_time = 0.5,
    result = "stone-age-furnace"
}
stone_furnace.placeable_by = {item = "stone-age-furnace", count = 1}
data:extend({table.deepcopy(stone_furnace)})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- copper template

local speed = 0.5
local animspeed = 1
local energy = get_scaled_energy_usage(speed, 0)
local copper = {
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
            filename = string.format("%s/%s.ogg", DIR.sound_path, "grinder"),
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
    entity_info_icon_shift = standard_grinder_info_icon_shift(),
}

------------------------------------------------------------------------------------------------------------------------------------------------------

-- copper scrapper

local copper_scrapper = table.deepcopy(copper)
copper_scrapper.name = "copper-scrapper"
copper_scrapper.type = "furnace"
copper_scrapper.source_inventory_size = 1
copper_scrapper.result_inventory_size = 5
copper_scrapper.icon = get_icon_path("copper-scrapper", DIR.icon_size)
copper_scrapper.icon_size = DIR.icon_size
copper_scrapper.crafting_categories = {"scrapping"}
copper_scrapper.energy_source.emissions_per_minute = 24
copper_scrapper.match_animation_speed_to_activity = false
copper_scrapper.animation = {
    layers = {
        get_layer("machines/copper-scrapper-base", 30, 5, false, nil, anim_speed, 192, 192, 0, 0, 192, 192, {0,0}),
        get_layer("machines/copper-shadow", nil, nil, true, 30, anim_speed, 256, 192, 0, 0, 256, 192, {0.5,0}),
    }
}
copper_scrapper.working_visualisations = {
    {
        animation = get_layer("machines/bronze-grinder-working", 30, 5, false, nil, animspeed, 64, 64, 0, 0, 64, 64, {0,1}, "additive"),
    }
}
copper_scrapper.placeable_by = {item = "copper-scrapper", count = 1}
copper_scrapper.minable = {
    mining_time = 0.5,
    result = "copper-scrapper"
}
copper_scrapper.fast_replaceable_group = "scrapper"
data:extend({copper_scrapper})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- watchtower

local animspeed = 1
local energy = get_scaled_energy_usage(0.5, 0)

data:extend({
    {
        name = "watchtower",
        type = "radar",
        collision_box = {{-0.7,-0.7},{0.7,0.7}},
        selection_box = {{-1,-1},{1,1}},
        corpse = "medium-remnants",
        energy_per_nearby_scan = "1YJ", -- prevents nearby scanning
        energy_per_sector = "8MJ",
        energy_source = {
			effectivity = 1,
			emissions_per_minute = 3,
			fuel_category = "chemical",
			fuel_inventory_size = 1,
			type = "burner"
        },
        energy_usage = energy.active,
        flags = {
            "placeable-player",
            "player-creation"
        },
        icon = get_icon_path("watchtower"),
        icon_size = DIR.icon_size,
        max_distance_of_nearby_sector_revealed = 0,
        max_distance_of_sector_revealed = 14,
        max_health = 150,
        minable = {
            mining_time = 0.5,
            result = "watchtower"
        },
        pictures = {
            layers = {
				get_layer("machines/stone-tower-base", 64, 8, false, nil, anim_speed, 128, 192, 0, 0, 128, 192, {0,-0.5}, nil, nil, nil, 64),
				get_layer("machines/stone-tower-shadow", 64, 8, true, nil, anim_speed, 288, 96, 0, 0, 288, 96, {1.25,0.25}, nil, nil, nil, 64),
			}
        },
        radius_minimap_visualisation_color = {
            a = 0.275,
            b = 0.235,
            g = 0.092,
            r = 0.059
        },
        resistances = {
            {
                percent = 75,
                type = "fire"
            },
        },
        rotation_speed = 1/120,
        vehicle_impact_sound = {
            filename = "__base__/sound/car-stone-impact.ogg",
            volume = 0.65
        },
        working_sound = {
            sound = {
                {
                    filename = "__base__/sound/furnace.ogg",
					volume = 1,
                }
            }
        }
    }
})

-----------------------------------------------------------------------------------------------------------------------------------------------------
