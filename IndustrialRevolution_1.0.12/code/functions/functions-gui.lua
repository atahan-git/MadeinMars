------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.data.globals")
require("code.data.displays")
local displays = get_displays()
local display_columns = 12
local display_rows = 9
local variation_offset = 2

------------------------------------------------------------------------------------------------------------------------------------------------------

function gui_toggle(event)
    local player = game.players[event.player_index]
    local frame = player.gui.screen[DIR.display_frame]
    if frame then
        return frame.destroy()
    end
end

function get_map_markers(entity)
	return entity.force.find_chart_tags(entity.surface, entity.bounding_box)
end

function get_tile_bounds(position)
	return {{position.x - 0.5, position.y - 0.5},{position.x + 0.5, position.y + 0.5}}
end

function add_marker(entity, icon)
	entity.force.add_chart_tag(entity.surface, { icon = { type = icon.type, name = icon.name}, position = entity.position })
end

function get_has_marker_flag(entity)
	return (entity.color.r == 1)
end

function set_has_marker_flag(entity, bool)
	entity.color.r = (bool and 1 or 0)
end

function remove_markers(entity)
	if entity and entity.valid then
		local markers = get_map_markers(entity)
		for _,marker in pairs(markers) do
			marker.destroy()
		end
	end
end

function create_display_gui(player, selected)
    -- display buttons
    -- if refresh or event.gui_type == defines.gui_type.entity then
    if player and selected then
		global.last_display[player.index] = selected
		local markers = next(get_map_markers(selected)) ~= nil
		local frame = player.gui.screen[DIR.display_frame]
		if frame then frame.destroy() end
		player.opened = player.gui.screen
		-- frame
		frame = player.gui.screen.add {
			type = "frame",
			name = DIR.display_frame,
			direction = "vertical",
			style = "display_frame",
		}
		frame.force_auto_center()
		-- frame header
		local header = frame.add {
			type = "flow",
			direction = "horizontal",
		}
		header.style.vertical_align = "center"
		header.add {
			type = "label",
			caption = {"entity-name."..selected.name},
			style = "frame_title",
		}
		local filler = header.add {
			type = "empty-widget",
			style = "draggable_space_header",
		}
		filler.style.natural_height = 24
		filler.style.horizontally_stretchable = true
		filler.drag_target = frame
		header.add {
			name = "display-header-close",
			type = "sprite-button",
			style = "display_small_button",
			sprite = "utility/close_white",
			tooltip = {"controls.close-gui"},
		}
		-- main body of frame
		-- table wrapper
		local inner_table = frame.add {
			type = "table",
			name = "inner-table",
			column_count = 2,
			vertical_centering = false,
		}
		inner_table.style.horizontal_spacing = 8
		-- left column
		local left_column = inner_table.add {
			type = "flow",
			name = "left-column",
			direction = "vertical",
		}
		left_column.style.width = 120
		left_column.style.horizontal_align = "center"
		-- camera frame
		local inner_frame_left = left_column.add {
			type = "frame",
			name = "inner-frame-left",
			style = "inside_deep_frame",
			direction = "vertical",
		}
		local camera = inner_frame_left.add {
			type = "camera",
			position = { x = selected.position.x, y = selected.position.y },
			zoom = player.display_scale,
		}
		camera.style.width = 120
		camera.style.height = 120
		-- map marker button
		local map_button = left_column.add {
			name = "display-header-map-marker",
			type = "sprite-button",
			style = markers and "display_map_button_active" or "display_map_button",
			sprite = "display-map-marker",
		}
		map_button.style.top_margin = 8
		map_button.style.bottom_margin = 4
		map_button.enabled = (selected.graphics_variation ~= 1)
		left_column.add {
			type = "label",
			caption = {"controls.display-map-marker"},
		}
		-- main gui buttons area frame
		local right_column = inner_table.add {
			type = "flow",
			direction = "vertical",
		}
		local inner_frame_right = right_column.add {
			type = "frame",
			style = "inside_deep_frame",
			direction = "vertical",
		}
		inner_frame_right.style.width = display_columns * 40
		inner_frame_right.style.minimal_height = 120
		inner_frame_right.style.maximal_height = 400
		-- symbol selection buttons
		local buttons = inner_frame_right.add {
			type = "table",
			name = "display-buttons",
			column_count = display_columns,
			style = "display_buttons",
		}
		-- first make "rubber grid" and store buttons in an array
		local button_proto = {}
		for row = 1,display_rows do
			button_proto[row] = {}
			for column = 1,display_columns do
				button_proto[row][column] = {
					type = "sprite-button",
					style = "display_button",
				}
			end
		end
		-- replace grid with indexed symbols
		for index,icon in ipairs(displays) do
			if icon.name ~= "blank" then
				if icon.row > 0 and icon.column > 0 then
					button_proto[icon.row][icon.column].name = "display-symbol-" .. index
					button_proto[icon.row][icon.column].sprite = icon.type .. "/" .. icon.name
					button_proto[icon.row][icon.column].tooltip = {icon.type .. "-name." .. icon.name}
					button_proto[icon.row][icon.column].style = (index == selected.graphics_variation) and "display_button_selected" or "display_button"
					button_proto[icon.row][icon.column].index = index -- not stored in prototype
				end
			end
		end
		-- create the grid
		for row = 1,display_rows do
			for column = 1,display_columns do
				local button = buttons.add(button_proto[row][column])
				button.style.height = button_proto[row][column].name and 40 or 20
				button.style.width = button_proto[row][column].name and 40 or 20
				button.style.margin = button_proto[row][column].name and 0 or 10
				button.ignored_by_interaction = (button_proto[row][column].name == nil) or (button_proto[row][column].index == selected.graphics_variation)
			end
		end
    end
end

local display_gui_click = {
	["display-symbol"] = function (event, variation)
		local player = game.players[event.player_index]
		local last_display = global.last_display[player.index]
		if last_display then
			last_display.graphics_variation = variation
			local markers = get_map_markers(last_display)
			if markers and next(markers) ~= nil then
				remove_markers(last_display)
				add_marker(last_display, displays[variation])
			end
			for _,child in pairs(event.element.parent.children) do
				child.style = (child.name == "display-symbol-"..variation) and "display_button_selected" or "display_button"
				child.ignored_by_interaction = (child.name == "display-symbol-"..variation) or not string.find(child.name,"display")
			end
			player.gui.screen[DIR.display_frame]["inner-table"]["left-column"]["display-header-map-marker"].enabled = true
		end
	end,
    ["display-header-close"] = function (event)
        gui_toggle(event)
    end,
    ["display-header-map-marker"] = function (event)
		local player = game.players[event.player_index]
		local last_display = global.last_display[player.index]
		if last_display then 
			local markers = get_map_markers(last_display)
			if not markers or next(markers) == nil then
				event.element.style = "display_map_button_active"
				add_marker(last_display, displays[last_display.graphics_variation])
				last_display.color = {r=1}
			else
				event.element.style = "display_map_button"
				remove_markers(last_display)
				last_display.color = {r=0}
			end
		end
    end,
}

function gui_click(event)
	if display_gui_click[event.element.name] then
		display_gui_click[event.element.name](event)
		return
	end
	if string.find(event.element.name, "display%-symbol") then
		display_gui_click["display-symbol"](event, tonumber(string.match(event.element.name, "%d+")))
		return
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------
