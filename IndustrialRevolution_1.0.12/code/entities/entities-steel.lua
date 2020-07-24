------------------------------------------------------------------------------------------------------------------------------------------------------

-- steel template

local steel_light = { color = DIR.indicator_colours.steel, intensity = 0.5, size = 9, shift = {0,0.5} }

local speed = 1
local energy = get_scaled_energy_usage(1.5, 0.04)

local steel = {
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
        "pollution"
    },
    module_specification = {
        module_info_icon_shift = standard_entity_info_module_shift(),
        module_slots = 4
    },
    max_health = 600,
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

local animspeed = DIR.anim_speed/speed
local steel_light = { color = {1,1,1}, intensity = 0.5, size = 9, shift = {0,0.5}}
local glow = get_layer("machines/assembler-glow", 30, 5, false, nil, animspeed, 192, 128, 0, 0, 192, 128, {0,0}, "additive", nil, DIR.indicator_colours.steel)

------------------------------------------------------------------------------------------------------------------------------------------------------

-- steel washer

local ore_tint = get_layer("machines/steel-washer-ore-tint", 60, 10, false, nil, animspeed, 64, 64, 0, 0, 64, 64, {0.0,-0.5})
local ore_white = get_layer("machines/steel-washer-ore-white", 60, 10, false, nil, animspeed, 64, 64, 0, 0, 64, 64, {0.0,-0.5}, "additive")

local steel_washer = table.deepcopy(steel)
steel_washer.name = "steel-washer"
steel_washer.type = "assembling-machine"
steel_washer.icon = get_icon_path("steel-washer", DIR.icon_size)
steel_washer.icon_size = DIR.icon_size
steel_washer.crafting_categories = {"washing","advanced-washing"}
steel_washer.allowed_effects = {
	"consumption",
	"speed",
	"productivity",
	"pollution",
}
steel_washer.gui_title_key = "gui-title.washing"
steel_washer.animation = {
    layers = {
        get_layer("machines/steel-washer-base", 30, 5, false, nil, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}),
        get_layer("machines/iron-shadow", nil, nil, true, 30, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
    }
}
steel_washer.working_visualisations = {
    {
        animation = glow,
        light = steel_light,
    },
    {
        animation = ore_tint,
        apply_recipe_tint = "tertiary",
    },
    {
        animation = ore_white,
    },
}
steel_washer.placeable_by = {item = "steel-washer", count = 1}
steel_washer.minable = {
    mining_time = 0.5,
    result = "steel-washer"
}
steel_washer.fast_replaceable_group = "grinder"
steel_washer.working_sound.sound.filename = string.format("%s/%s.ogg", DIR.sound_path, "washer")
steel_washer.fluid_boxes = standard_fluid_boxes()
steel_washer.fluid_boxes.off_when_no_fluid_recipe = false
data:extend({table.deepcopy(steel_washer)})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- steel cleaner

local steel_cleaner = table.deepcopy(steel)
steel_cleaner.name = "steel-cleaner"
steel_cleaner.type = "assembling-machine"
steel_cleaner.icon = get_icon_path("steel-cleaner", DIR.icon_size)
steel_cleaner.icon_size = DIR.icon_size
steel_cleaner.crafting_categories = {"cleaning"}
steel_cleaner.animation = {
    layers = {
        get_layer("machines/steel-cleaner-base", 30, 5, false, nil, animspeed, 192, 256, 0, 0, 192, 256, {0,-0.5}),
        get_layer("machines/iron-shadow", nil, nil, true, 30, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
    }
}
steel_cleaner.working_visualisations = {
    {
        animation = glow,
        light = steel_light,
    }
}
steel_cleaner.placeable_by = {item = "steel-cleaner", count = 1}
steel_cleaner.minable = {
    mining_time = 0.5,
    result = "steel-cleaner"
}
steel_cleaner.fast_replaceable_group = "cleaner"
steel_cleaner.working_sound.sound.filename = string.format("%s/%s.ogg", DIR.sound_path, "water")
steel_cleaner.fluid_boxes = standard_fluid_boxes()
for _,fbox in pairs(steel_cleaner.fluid_boxes) do
    if type(fbox) == "table" then
        if fbox.production_type == "input" then fbox.filter = "dirty-water"
        elseif fbox.production_type == "output" then fbox.filter = "water" end
    end
end
steel_cleaner.fixed_recipe = "dirty-water-cleaning"
data:extend({table.deepcopy(steel_cleaner)})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- steel electroplater

energy = get_scaled_energy_usage(2, 0.04)

local steel_electroplater = table.deepcopy(steel_washer)
steel_electroplater.name = "steel-electroplater"
steel_electroplater.icon = get_icon_path("steel-electroplater", DIR.icon_size)
steel_electroplater.icon_size = DIR.icon_size
steel_electroplater.crafting_categories = {"electroplating"}
steel_electroplater.gui_title_key = "gui-title.electroplating"
steel_electroplater.energy_usage = energy.active
steel_electroplater.energy_source = {
	type = "electric",
	usage_priority = "secondary-input",
	drain = energy.passive,
	emissions_per_minute = 1,
}
steel_electroplater.animation = {
    layers = {
        get_layer("machines/steel-electroplater-base", 30, 5, false, nil, DIR.anim_speed, 192, 192, 0, 0, 192, 192, {0,0}),
        get_layer("machines/iron-shadow", nil, nil, true, 30, DIR.anim_speed, 256, 192, 0, 0, 256, 192, {0.5,0}),
    }
}
steel_electroplater.working_visualisations = {
	{
		animation = get_layer("machines/steel-electroplater-working", 30, 5, false, nil, DIR.anim_speed, 192, 192, 0, 0, 192, 192, {0,0}),
        apply_recipe_tint = "secondary",
	},
    {
        animation = glow,
        light = steel_light,
    }
}
steel_electroplater.placeable_by = {item = "steel-electroplater", count = 1}
steel_electroplater.minable = {
    mining_time = 0.5,
    result = "steel-electroplater"
}
steel_electroplater.fluid_boxes[2] = nil
steel_electroplater.fast_replaceable_group = "assembling-machine"
steel_electroplater.working_sound.sound.filename = string.format("%s/%s.ogg", DIR.sound_path, "electroplater")
steel_electroplater.match_animation_speed_to_activity = false
data:extend({table.deepcopy(steel_electroplater)})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- steel grinder

speed = 2
animspeed = DIR.anim_speed/speed
energy = get_scaled_energy_usage(2.5, 0.04)

local grinder = table.deepcopy(data.raw["assembling-machine"]["iron-grinder"])

grinder.name = "steel-grinder"
grinder.crafting_categories = {"grinding","grinding-1","grinding-2","grinding-3","grinding-4"}
grinder.icon = get_icon_path("steel-grinder", DIR.icon_size)
grinder.module_specification.module_slots = 2
grinder.max_health = 600
grinder.crafting_speed = speed
grinder.allowed_effects = {
	"consumption",
	"speed",
	"productivity",
	"pollution",
}
grinder.energy_usage = energy.active
grinder.energy_source = {
	type = "electric",
	usage_priority = "secondary-input",
	drain = energy.passive,
	emissions_per_minute = 3,
}
grinder.animation = {
    layers = {
        get_layer("machines/steel-grinder-base", 30, 5, false, nil, animspeed, 192, 192, 0, 0, 192, 192, {0,0}),
        get_layer("machines/iron-shadow", nil, nil, true, 30, animspeed, 256, 192, 0, 0, 256, 192, {0.5,0}),
    }
}
grinder.working_visualisations = {
    {
        animation = glow,
        light = steel_light,
    },
    {
        animation = get_layer("machines/steel-working-indicator", 20, 5, false, nil, animspeed, 64, 64, 0, 0, 64, 64, {0,1}, "additive"),
        apply_recipe_tint = "tertiary",
    },
}
grinder.placeable_by = {item = "steel-grinder", count = 1}
grinder.fast_replaceable_group = "grinder"
grinder.minable = {
    mining_time = 0.5,
    result = "steel-grinder"
}
grinder.entity_info_icon_shift = standard_grinder_info_icon_shift()

data:extend({grinder})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- arc furnace - not scaled by activity

speed = 4
energy = get_scaled_energy_usage(5, 0.04)

local arc_furnace = table.deepcopy(steel)
arc_furnace.name = "steel-age-furnace"
arc_furnace.type = "assembling-machine"
arc_furnace.icon = get_icon_path("steel-age-furnace", DIR.icon_size)
arc_furnace.icon_size = DIR.icon_size
arc_furnace.crafting_categories = {"smelting","smelting-1","smelting-2","smelting-3","smelting-4","smelting-5"}
arc_furnace.crafting_speed = speed
arc_furnace.allowed_effects = {
	"consumption",
	"speed",
	"productivity",
	"pollution",
}
arc_furnace.energy_usage = energy.active
arc_furnace.energy_source = {
	type = "electric",
	usage_priority = "secondary-input",
	drain = energy.passive,
	emissions_per_minute = speed * 3,
}
arc_furnace.gui_title_key = "gui-title.smelting"
arc_furnace.selection_box = {{-2.5,-2.5},{2.5,2.5}}
arc_furnace.collision_box = {{-2.25,-2.25},{2.25,2.25}}
arc_furnace.module_specification.module_slots = 2
arc_furnace.module_specification.module_info_icon_shift = {0,1.25}
arc_furnace.entity_info_icon_shift = {0,0.5}
arc_furnace.animation = {
    layers = {
        get_layer("machines/steel-furnace-base", nil, nil, false, nil, 1, 320, 384, 0, 0, 320, 384, {0,-0.5}),
		get_layer("machines/steel-furnace-shadow", nil, nil, true, nil, 1, 640, 384, 0, 0, 640, 384, {2.5,-0.5}),
    }
}
arc_furnace.working_visualisations = {
    {
        animation = get_layer("machines/steel-furnace-working", 60, 10, false, nil, 1, 320, 384, 0, 0, 320, 384, {0,-0.5}, "additive"),
        light = { color = {r=1,g=1,b=0.75}, intensity = 0.4, size = 12 },
    },
    {
        light = { color = {r=1,g=1,b=0.75}, intensity = 0.4, size = 12 },
		effect = "uranium-glow",
    },
}
arc_furnace.placeable_by = {item = "steel-age-furnace", count = 1}
arc_furnace.minable = {
    mining_time = 0.5,
    result = "steel-age-furnace"
}
arc_furnace.fast_replaceable_group = "arc-furnace"
arc_furnace.working_sound.sound.filename = string.format("%s/%s.ogg", DIR.base_sound_path, "electric-furnace")
arc_furnace.match_animation_speed_to_activity = false
data:extend({arc_furnace})

------------------------------------------------------------------------------------------------------------------------------------------------------
