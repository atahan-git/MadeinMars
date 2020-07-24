------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-maths")
require("code.data.displays")
require("code.functions.functions-gui")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- constants

local max_tips = 52
local tip_text_colour = "1.0,0.8,0.6"
local tip_interval = 36000 -- 10 minutes
local generator_interval = 900 -- 15 seconds

local default_font = "dir2-font"
local red_text = "[color=1,0.25,0.25]"
local grey_text = "[color=0.5,0.5,0.5]"
local generator_manager_stack_target = 10

local age = settings.global["deadlock-industry-starting-age"].value
local kit = settings.global["deadlock-industry-starting-kit"].value
local starter = require("code.data.kits")
local displays = get_displays()

local polluters = {
	["crude-oil"] = 0.005,
	["heavy-oil"] = 0.00167,
	["light-oil"] = 0.00167,
	["petroleum-gas"] = 0.00167,
	["lubricant"] = 0.002,
	["sulfuric-acid"] = 0.002,
	["chromic-acid"] = 0.002,
	["plastic-fluid"] = 0.002,
}

local turret_property_list = {"energy","direction","position","health","kills","damage_dealt"}

------------------------------------------------------------------------------------------------------------------------------------------------------

local function recursive_research(player, tech_name)
    if player.force.technologies[tech_name] and not player.force.technologies[tech_name].researched then
        player.force.technologies[tech_name].researched = true
        if player.force.technologies[tech_name].prerequisites then
            for _,prereq in pairs(player.force.technologies[tech_name].prerequisites) do
                recursive_research(player, prereq.name)
            end
        end
    end
end

local function print_wrapped_locale(player_or_game, locale)
    player_or_game.print({"", string.format("[color=%s][font=%s]", tip_text_colour, default_font), locale, "[/font][/color]"})
end

local function print_wrapped_next_tip(player)
	local tip = global.player_tips[player.index] or 1
    print_wrapped_locale(player, {string.format("tips.%03d", tip)})
	global.player_tips[player.index] = (tip < max_tips) and (tip + 1) or 1
end

local function register_commands()
    commands.remove_command("tip")
    commands.add_command("tip", {"deadlock-industry-commands.tip"}, function(data)
        print_wrapped_next_tip(game.players[data.player_index])
    end)
end

local function call_freeplay()
	if remote.interfaces["freeplay"] then
		remote.call("freeplay", "set_skip_intro", true)
		remote.call("freeplay", "set_respawn_items", {})
		remote.call("freeplay", "set_created_items", kit and starter.kits[age] or starter.kits["default"])
	end
end

local function call_siloscript()
	if remote.interfaces["silo_script"] then
		remote.call("silo_script", "add_tracked_item", "ion-probe")
		remote.call("silo_script", "add_tracked_item", "impulse-probe")
	end
end

local function initialise_globals()
	if global.player_tips == nil then global.player_tips = {} end
	if global.last_display == nil then global.last_display = {} end
end

local function initialise()
	initialise_globals()
	call_freeplay()
	call_siloscript()
    register_commands()
end

-- migration/refresh
local function config_refresh(event)
	local changes = event.mod_changes["IndustrialRevolution"]
	if changes and changes.new_version ~= changes.old_version then
		initialise_globals()
		call_freeplay()
		call_siloscript()
		print_wrapped_locale(game, {"tips.update", changes.new_version})
		for _,force in pairs(game.forces) do
			force.reset_recipes()
			force.reset_technologies()
			force.reset_technology_effects()
		end
	end
end

-- manage generator equipment
local function refuel(player)
    local inventory = player.get_inventory(defines.inventory.character_armor)
    if not inventory then return end
    local armour = inventory[1]
    if armour and armour.valid_for_read and armour.grid and armour.grid.equipment then
        local used = {}
		local warnings = ""
        for _,equipment in pairs(armour.grid.equipment) do
            if equipment.burner then
                if equipment.burner.inventory then
                    for i=1,#equipment.burner.inventory do
                        local item = equipment.burner.inventory[i]
                        if item.valid_for_read and item.count < generator_manager_stack_target then
							local removed = player.get_main_inventory().remove({name=item.name, count=generator_manager_stack_target-item.count})
							if removed > 0 then
								if not used[item.name] then used[item.name] = 0 - removed
								else used[item.name] = used[item.name] - removed end
								equipment.burner.inventory.insert({name=item.name, count=removed})
							else
								if settings.get_player_settings(player)["deadlock-industry-genman-pingnofuel"].value then warnings = warnings .. (warnings == "" and "" or " ") .. "[img=fuel-icon-yellow]" end
							end
						elseif not item.valid_for_read then
							if settings.get_player_settings(player)["deadlock-industry-genman-pingemptyslot"].value then warnings = warnings .. (warnings == "" and "" or " ") .. "[img=fuel-icon-red]" end
                        end
                    end
                end
				if equipment.burner.burnt_result_inventory then
                    for i=1,#equipment.burner.burnt_result_inventory do
                        local item = equipment.burner.burnt_result_inventory[i]
                        if item.valid_for_read and item.count >= 1 then
							local inserted = player.get_main_inventory().insert({name=item.name, count=item.count})
							if inserted > 0 then
								if not used[item.name] then used[item.name] = inserted
								else used[item.name] = used[item.name] + inserted end
								equipment.burner.burnt_result_inventory.remove({name=item.name, count=inserted})
							else
								if settings.get_player_settings(player)["deadlock-industry-genman-pingnoinv"].value then warnings = warnings .. (warnings == "" and "" or " ") .. "[img=fuel-icon-inventory]" end
							end
                        end
                    end
				end
			end
        end
		local text = ""
		for item,amount in pairs(used) do
			text = text .. (text == "" and "" or " ") .. "[img=item/"..item.."] " .. (amount >= 0 and grey_text.."+" or red_text) .. amount .. "[/color]"
		end
		if text ~= "" and settings.get_player_settings(player)["deadlock-industry-genman-inventorysounds"].value then
			player.play_sound{path = "genman-inventory"}
		end
		if warnings ~= "" then
			if text ~= "" then text = text .. " " end
			text = text .. warnings
			if settings.get_player_settings(player)["deadlock-industry-genman-pingsounds"].value then
				player.play_sound{path = "genman-warning"}
			end
		end
		if text ~= "" then player.create_local_flying_text{text=text, position={player.position.x,player.position.y-2}} end
    end
end

local function player_has_a_generator(player)
    local armour = player.get_inventory(defines.inventory.character_armor) and player.get_inventory(defines.inventory.character_armor)[1]
    if armour and armour.valid_for_read and armour.grid and armour.grid.equipment then
        for _,equipment in pairs(armour.grid.equipment) do
            if equipment.burner then return true end
        end
    end
    return false
end

local function is_a_display(entity)
	return (entity.name == "bronze-display" or entity.name == "iron-display" or entity.name == "gold-display")
end

local function get_display_filters()
	return {
		{filter = "name", name = "bronze-display"},
		{filter = "name", name = "iron-display"},
		{filter = "name", name = "gold-display"},
		{filter = "type", type = "storage-tank"},
	}
end

local function tank_destroyed(entity, player)
	local pollution = 0
	for fluid,amount in pairs(entity.get_fluid_contents()) do
		if polluters[fluid] then pollution = pollution + (polluters[fluid] * amount) end
	end
	if pollution > 0 then
		entity.surface.pollute(entity.position, pollution)
		if player then player.create_local_flying_text{text="[img=tank-pollution] "..red_text.."+"..math.floor(pollution).."[/color]", position=entity.position} end
	end
end

local function event_raised_destroy(event)
    if event.entity and event.entity.valid then
		local player = event.player_index and game.players[event.player_index] or nil
		-- map markers
		if is_a_display(event.entity) then
			remove_markers(event.entity)
		-- storage tanks
		elseif event.entity.type == "storage-tank" then
			tank_destroyed(event.entity, player)
		end
    end
end

local function player_cannot_reach(player,entity)
	player.play_sound{path = "utility/cannot_build"}
	player.create_local_flying_text{text={"cant-reach"}, position=entity.position}
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- EVENTS

-- initialisation
script.on_init(initialise)
script.on_load(register_commands)
script.on_configuration_changed(config_refresh)

-- welcome and init player
script.on_event(defines.events.on_player_created, function(event)
    local player = game.players[event.player_index]
    if player and player.admin and starter.tech[age] then
		for _,tech in pairs(starter.tech[age]) do
			recursive_research(player, tech)
		end
    end
    print_wrapped_locale(player, {"tips.modname"})
    print_wrapped_next_tip(player)
    player.set_shortcut_available("deadlock-industry-generator-manager", false)
end)

-- tips
script.on_nth_tick(tip_interval, function(event)
    if event.tick == 0 then return end
    for _,player in pairs(game.players) do
        local freq = tonumber(settings.get_player_settings(player)["deadlock-industry-tip-frequency"].value) / 10
        if freq and event.tick % (tip_interval*freq) == 0 then
            print_wrapped_next_tip(player)
        end
    end
end)

-- restock equipment grid generators
script.on_nth_tick(generator_interval, function(event)
    for _,player in pairs(game.players) do
        if player.is_shortcut_available("deadlock-industry-generator-manager") and player.is_shortcut_toggled("deadlock-industry-generator-manager") then
            refuel(player)
        end
    end
end)

-- generator manager quickbar shortcut
script.on_event(defines.events.on_lua_shortcut, function(event)
    if event.prototype_name == "deadlock-industry-generator-manager" then
        local player = game.players[event.player_index]
        player.set_shortcut_toggled("deadlock-industry-generator-manager", not player.is_shortcut_toggled("deadlock-industry-generator-manager"))
    end
end)

-- generator manager hotkey
script.on_event("deadlock-control-generator-manager", function(event)
	local player = game.players[event.player_index]
    if player and player.is_shortcut_available("deadlock-industry-generator-manager") then
        local player = game.players[event.player_index]
        player.set_shortcut_toggled("deadlock-industry-generator-manager", not player.is_shortcut_toggled("deadlock-industry-generator-manager"))
    end
end)

-- generator manager equipment detection
script.on_event({defines.events.on_player_placed_equipment, defines.events.on_player_removed_equipment, defines.events.on_player_armor_inventory_changed}, function(event)
    local player = game.players[event.player_index]
    if player then player.set_shortcut_available("deadlock-industry-generator-manager", player_has_a_generator(player)) end
end)

-- display plate gui
script.on_event("deadlock-open-gui", function(event)
    local player = game.players[event.player_index]
	if player.cursor_stack and player.cursor_stack.valid_for_read then return end
    local selected = player and player.selected
    if selected and selected.valid and is_a_display(selected) then 
		if player.can_reach_entity(selected) then
			create_display_gui(player, selected)
		else
			player_cannot_reach(player, selected)
		end
	end
end)
script.on_event(defines.events.on_gui_closed, gui_toggle)
script.on_event(defines.events.on_gui_click, gui_click)

-- display plate C&P
script.on_event(defines.events.on_entity_settings_pasted, function (event)
	if event.destination and event.destination.valid and event.source and event.source.valid and is_a_display(event.destination) and is_a_display(event.source) then
		remove_markers(event.destination)
		if get_has_marker_flag(event.source) then
			set_has_marker_flag(event.destination, true)
			add_marker(event.destination, displays[event.destination.graphics_variation])
		else
			set_has_marker_flag(event.destination, false)
		end
	end
end)

-- bots building displays
script.on_event(defines.events.on_robot_built_entity, function (event)
	if event.created_entity and event.created_entity.valid and is_a_display(event.created_entity) and get_has_marker_flag(event.created_entity) then
		add_marker(event.created_entity, displays[event.created_entity.graphics_variation])
	end
end, get_display_filters())

-- tidy up map markers + deal with storage tanks
script.on_event(defines.events.on_player_mined_entity, event_raised_destroy, get_display_filters())
script.on_event(defines.events.on_robot_mined_entity, event_raised_destroy, get_display_filters())
script.on_event(defines.events.on_entity_died, event_raised_destroy, get_display_filters())

-- other gui catches
script.on_event(defines.events.on_gui_opened, function(event)
    local player = game.players[event.player_index]
    local selected = player and player.selected
    if selected and selected.name == "flare-stack" then player.opened = nil end
end)

-- research completion
script.on_event(defines.events.on_research_finished, function(event)
	-- punkbot research unlocks quickbar stuff because can't be done in shortcut prototype for more than one research
	if event.research and event.research.name == "deadlock-bronze-construction" then
		for _,player in pairs(event.research.force.players) do
			for _,shortcut in pairs(game.shortcut_prototypes) do
				if shortcut.technology_to_unlock == "personal-roboport-equipment" then player.set_shortcut_available(shortcut.name,true) end
			end
		end
	end
	-- research notifications
    local recipes = ""
    if event.research.effects then
        for _,effect in pairs(event.research.effects) do
            if effect.type == "unlock-recipe" then recipes = recipes .. "[img=recipe/" .. effect.recipe .. "] " end
        end
    end
	for _,player in pairs(event.research.force.players) do
		if settings.get_player_settings(player)["deadlock-industry-research-notifications"].value then
			player.print({"", "[color=192,255,192]", {"notification.research-complete", event.research.name, event.research.localised_name, recipes}, "[/color]"})
		end
	end
end)

------------------------------------------------------------------------------------------------------------------------------------------------------

-- inserter adjustment

local function adjust_position(inserter, position_type, increment, maximum, reset)
    local target = get_vector_difference(inserter[position_type], inserter.position)
    if target.x < 0 then
        if target.x > -maximum then inserter[position_type] = get_vector_sum( inserter[position_type], { x = -increment, y = 0} )
        else inserter[position_type] = get_vector_sum( inserter[position_type], { x = reset, y = 0} ) end
    elseif target.x > 0 then
        if target.x < maximum then inserter[position_type] = get_vector_sum( inserter[position_type], { x = increment, y = 0} )
        else inserter[position_type] = get_vector_sum( inserter[position_type], { x = -reset, y = 0} ) end
    elseif target.y < 0 then
        if target.y > -maximum then inserter[position_type] = get_vector_sum( inserter[position_type], { x = 0, y = -increment } )
        else inserter[position_type] = get_vector_sum( inserter[position_type], { x = 0, y = reset } ) end
    elseif target.y > 0 then
        if target.y < maximum then inserter[position_type] = get_vector_sum( inserter[position_type], { x = 0, y = increment } )
        else inserter[position_type] = get_vector_sum( inserter[position_type], { x = 0, y = -reset } ) end
    end
end

if settings.startup["deadlock-industry-inserter-adjustment"].value then
    script.on_event("deadlock-inserter-dropoff", function(event)
        local player = game.players[event.player_index]
        if player and player.selected and player.selected.type == "inserter" then
			if player.can_reach_entity(player.selected) then
				local maximum = string.find(player.selected.name,"long%-handed") and 2 or 1
				adjust_position(player.selected, "drop_position", 0.5, maximum, 0.5)
				player.play_sound{path="inserter-squeak"}
			else
				player_cannot_reach(player,player.selected)
			end
        end
    end)
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- photonic cannon adjustment

script.on_event("deadlock-turret-angle-toggle", function(event)
	local player = game.players[event.player_index]
	if player and player.selected and player.selected.valid and player.selected.force == player.force then
		local properties = {}
		if (player.selected.name == "photon-turret") or (player.selected.name == "photon-turret-narrow") then
			if player.can_reach_entity(player.selected) then
				properties.name = (player.selected.name == "photon-turret") and "photon-turret-narrow" or "photon-turret"
				properties.sound = (player.selected.name == "photon-turret") and "deactivate" or "activate"
				for _,property in pairs(turret_property_list) do
					properties[property] = player.selected[property]
				end
			else
				player_cannot_reach(player,player.selected)
				return
			end
		end
		if properties.name then
			player.selected.destroy()
			local new = player.surface.create_entity{name=properties.name, position=properties.position, direction=properties.direction, force=player.force, player=player, raise_built=false, create_build_effect_smoke=false}
			if new and new.valid then
				new.energy = properties.energy
				new.health = properties.health
				new.kills = properties.kills or 0
				new.damage_dealt = properties.damage_dealt or 0
				player.play_sound{path=properties.sound}
			end
		end
	end
end)

------------------------------------------------------------------------------------------------------------------------------------------------------

-- rotation sound

script.on_event(defines.events.on_player_rotated_entity, function(event)
	local player = game.players[event.player_index]
	if player and event.entity and event.entity.valid then
		player.play_sound({path="rotate", volume_modifier=0.75})
	end
end)

------------------------------------------------------------------------------------------------------------------------------------------------------
