------------------------------------------------------------------------------------------------------------------------------------------------------

-- less is more
data.raw["utility-constants"]["default"].item_outline_radius = 8
data.raw["utility-constants"]["default"].item_outline_color = { 0, 0, 0, 0.75 }
data.raw["gui-style"]["default"].tooltip_title_label.minimal_width = 20

function update_utility_sprite(name,icon)
	if data.raw["utility-sprites"]["default"][name] then
		data.raw["utility-sprites"]["default"][name].filename = get_icon_path(icon)
		data.raw["utility-sprites"]["default"][name].height = DIR.icon_size
		data.raw["utility-sprites"]["default"][name].width = DIR.icon_size
	end
end

update_utility_sprite("slot_icon_module","module-slot")
update_utility_sprite("slot_icon_robot","robot-slot")
update_utility_sprite("slot_icon_robot_material","steel-repair-tool")
update_utility_sprite("slot_icon_ammo","ammo-slot")
update_utility_sprite("ammo_damage_modifier_icon","damage")
update_utility_sprite("default_ammo_damage_modifier_icon","damage")
update_utility_sprite("default_turret_attack_modifier_icon","damage")
update_utility_sprite("turret_attack_modifier_icon","damage")
update_utility_sprite("default_gun_speed_modifier_icon","rate-of-fire")
update_utility_sprite("gun_speed_modifier_icon","rate-of-fire")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- custom styles

local function add_styles(styles)
    local default_styles = data.raw["gui-style"]["default"]
    for name, style in pairs(styles) do
        default_styles[name] = style
    end
end

add_styles({
	display_frame = {
		type = "frame_style",
		parent = "frame",
		bottom_padding = 8,
	},
    display_buttons = {
        type = "table_style",
        horizontal_spacing = 0,
        vertical_spacing = 0,
    },
	display_button = {
		type = "button_style",
		parent = "quick_bar_slot_button",
		left_click_sound = {{ filename = DIR.core_sound_path.."/list-box-click.ogg", volume = 1 }},
	},
	display_button_selected = {
		type = "button_style",
		parent = "quick_bar_slot_button",
		default_graphical_set = data.raw["gui-style"]["default"]["quick_bar_slot_button"].selected_graphical_set
	},
	display_fake_header = {
		type = "frame_style",
		height = 24,
		graphical_set = data.raw["gui-style"]["default"]["draggable_space"].graphical_set,
		use_header_filler = false,
		horizontally_stretchable = "on",
		vertical_align = "center",
		alignment = "right",
		left_margin = data.raw["gui-style"]["default"]["draggable_space"].left_margin,
		right_margin = data.raw["gui-style"]["default"]["draggable_space"].right_margin,
	},
	display_small_button = {
		type = "button_style",
		parent = "close_button",
		left_margin = 1,
		right_margin = 1,
	},
	display_large_button = {
		type = "button_style",
		parent = "close_button",
		left_margin = 1,
		right_margin = 1,
		width = 40,
		height = 40,
		padding = 0,
	},
	display_map_button = {
		type = "button_style",
		parent = "display_large_button",
		left_click_sound = {{ filename = DIR.sound_path.."/activate.ogg", volume = 1 }},
	},
	display_map_button_active = {
		type = "button_style",
		parent = "display_large_button",
		left_click_sound = {{ filename = DIR.sound_path.."/deactivate.ogg", volume = 1 }},
		default_graphical_set = data.raw["gui-style"]["default"]["frame_button"].clicked_graphical_set,
	}
})

-- button sprites, genman sounds, genman icons

data:extend({
	{
		 type = "sprite",
		 name = "display-map-marker",
		 filename = get_icon_path("map-marker",64),
		 priority = "extra-high",
		 width = 64,
		 height = 64,
	},
    {
		type = "sprite",
		name = "fuel-icon-red",
		filename = get_icon_path("fuel-icon-red"),
		priority = "extra-high",
		width = 64,
		height = 64,
    },	
    {
		type = "sprite",
		name = "fuel-icon-yellow",
		filename = get_icon_path("fuel-icon-yellow"),
		priority = "extra-high",
		width = 64,
		height = 64,
    },	
    {
		type = "sprite",
		name = "fuel-icon-inventory",
		filename = get_icon_path("fuel-icon-inventory"),
		priority = "extra-high",
		width = 64,
		height = 64,
    },	
    {
		type = "sprite",
		name = "tank-pollution",
		filename = get_icon_path("pollution"),
		priority = "extra-high",
		width = 64,
		height = 64,
    },	
    {
		type = "sprite",
		name = "keyboard",
		filename = get_icon_path("keyboard"),
		priority = "extra-high",
		width = 64,
		height = 64,
    },	
	{
		type = "sprite",
		name = "tooltip-category-battery",
		filename = get_icon_path("tooltip-category-battery",40),
		priority = "extra-high-no-scale",
		width = 40,
		height = 40,
		flags = {"gui-icon"},
		scale = 0.5
	},
    {
		type = "sound",
		name = "genman-inventory",
		variations = {
			filename = DIR.sound_path.."/genman-inventory.ogg",
			volume = 1
		}
    },
    {
		type = "sound",
		name = "genman-warning",
		variations = {
			filename = DIR.sound_path.."/genman-warning.ogg",
			volume = 1
		}
    },	
    {
		type = "sound",
		name = "inserter-squeak",
		variations = {
			filename = DIR.sound_path.."/squeak.ogg",
			volume = 1
		}
    },	
    {
		type = "sound",
		name = "activate",
		variations = {
			filename = DIR.sound_path.."/activate.ogg",
			volume = 1
		}
    },	
    {
		type = "sound",
		name = "deactivate",
		variations = {
			filename = DIR.sound_path.."/deactivate.ogg",
			volume = 1
		}
    },	
    {
		type = "sound",
		name = "rotate",
		variations = {
			filename = DIR.sound_path.."/rotate.ogg",
			volume = 1
		}
    },	
})

------------------------------------------------------------------------------------------------------------------------------------------------------
