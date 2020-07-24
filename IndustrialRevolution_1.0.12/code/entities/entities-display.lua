------------------------------------------------------------------------------------------------------------------------------------------------------

-- displays

require("code.data.displays")

------------------------------------------------------------------------------------------------------------------------------------------------------

local function get_display_variation(name, material)
    local layer = {
        layers = {
            {
                filename = string.format("%s/machines/display-shadow.png", DIR.sprites_path),
                priority = "high",
                shift = {0,0},
                height = 192,
                width = 192,
                scale = 0.5,
				draw_as_shadow = true,
            },
            {
                filename = string.format("%s/machines/"..material.."-display-base.png", DIR.sprites_path),
                priority = "high",
                shift = {0,0},
                height = 192,
                width = 192,
                scale = 0.5,
            },
        }
    }
	if name ~= "blank" then
		table.insert(layer.layers,
			{
                filename = get_icon_path(name, 128),
                priority = "high",
                shift = {0,0},
                height = 128,
                width = 128,
                scale = 0.5,
            }
		)
	end
	return layer
end

------------------------------------------------------------------------------------------------------------------------------------------------------

local display =	{
	name = "bronze-display",
	type = "simple-entity-with-owner",
	localised_description = {"entity-description.display"},
	render_layer = "lower-object",
	icon_size = DIR.icon_size,
	corpse = "medium-remnants",
	minable = {
		mining_time = 0.1,
	},
	max_health = 100,
	flags = {"placeable-player", "placeable-neutral", "player-creation"},
	collision_box = standard_3x3_collision(),
	selection_box = standard_3x3_selection(),
	collision_mask = {
		"object-layer",
		"water-tile",
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
		filename = "__core__/sound/deconstruct-medium.ogg"
	},		
	resistances = {
	  {
		type = "fire",
		percent = 50
	  },
	},
	random_variation_on_create = false,
}

for _,material in pairs({"bronze","iron","gold"}) do
	local pictures = {}
	for _,icon in pairs(get_displays()) do
		table.insert(pictures, get_display_variation(icon.name, material))
	end
	display.pictures = pictures
	display.name = material.."-display"
	display.icon = get_icon_path(material.."-display")
	display.minable.result = material.."-display"
	data:extend({table.deepcopy(display)})
end

------------------------------------------------------------------------------------------------------------------------------------------------------
