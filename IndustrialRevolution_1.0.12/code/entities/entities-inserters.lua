------------------------------------------------------------------------------------------------------------------------------------------------------

-- make inserters adjustable

if settings.startup["deadlock-industry-inserter-adjustment"].value then
    for _,inserter in pairs(data.raw.inserter) do
        inserter.allow_custom_vectors = true
        inserter.localised_description = {"entity-description.deadlock-adjust-inserters"}
    end
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- burner inserter buff

data.raw.inserter["burner-inserter"].extension_speed = data.raw.inserter["inserter"].extension_speed
data.raw.inserter["burner-inserter"].rotation_speed = data.raw.inserter["inserter"].rotation_speed
data.raw.inserter["burner-inserter"].allow_burner_leech = settings.startup["deadlock-industry-enable-burner-leech"].value

-- red inserter nerf

data.raw.inserter["long-handed-inserter"].rotation_speed = data.raw.inserter["inserter"].rotation_speed

-- put vanilla filter inserters into same upgrade group

data.raw.inserter["filter-inserter"].fast_replaceable_group = "filter-inserter"
data.raw.inserter["filter-inserter"].next_upgrade = "stack-filter-inserter"
data.raw.inserter["stack-filter-inserter"].fast_replaceable_group = "filter-inserter"
data.raw.inserter["stack-filter-inserter"].filter_count = 5

------------------------------------------------------------------------------------------------------------------------------------------------------

-- long burner-inserter

local inserter = table.deepcopy(data.raw.inserter["burner-inserter"])

inserter.name = "long-handed-burner-inserter"
inserter.energy_per_rotation = "65KJ"
inserter.insert_position = data.raw.inserter["long-handed-inserter"].insert_position
inserter.pickup_position = data.raw.inserter["long-handed-inserter"].pickup_position
inserter.extension_speed = data.raw.inserter["long-handed-inserter"].extension_speed
inserter.rotation_speed = data.raw.inserter["long-handed-inserter"].rotation_speed
inserter.minable.result = "long-handed-burner-inserter"
inserter.fast_replaceable_group = "long-handed-inserter"
inserter.next_upgrade = "long-handed-inserter"
inserter.hand_size = 1.5
inserter.icon = get_icon_path("long-handed-burner-inserter")
inserter.icon_size = DIR.icon_size

data:extend({inserter})

-------------------------------------------------------------------------------------

-- orange filter inserter

local inserter = table.deepcopy(data.raw.inserter["inserter"])
inserter.name = "slow-filter-inserter"
inserter.icon = get_icon_path("slow-filter-inserter")
inserter.icon_size = DIR.icon_size
inserter.filter_count = 5
inserter.fast_replaceable_group = "filter-inserter"
inserter.next_upgrade = "filter-inserter"
inserter.minable.result = "slow-filter-inserter"

inserter.platform_picture = {
    sheet = {
        filename = DIR.sprites_path.."/inserters/hr-orange-inserter-platform.png",
        height = 79,
        priority = "extra-high",
        scale = 0.5,
        shift = {
            0.046875,
            0.203125
        },
		width = 105
    }
}
inserter.hand_base_picture = {
    filename = DIR.sprites_path.."/inserters/hr-orange-inserter-hand-base.png",
    height = 136,
    priority = "extra-high",
    scale = 0.25,
    width = 32
}
inserter.hand_closed_picture = {
    filename = DIR.sprites_path.."/inserters/hr-orange-inserter-hand-closed.png",
    height = 164,
    priority = "extra-high",
    scale = 0.25,
    width = 72
}
inserter.hand_open_picture = {
    filename = DIR.sprites_path.."/inserters/hr-orange-inserter-hand-open.png",
    height = 164,
    priority = "extra-high",
    scale = 0.25,
    width = 72
}

data:extend({inserter})

-------------------------------------------------------------------------------------

-- add type info overlays to icons

for _,inserter in pairs(data.raw.inserter) do
	local icon = inserter.filter_count and get_icon_path("sensor") or ((inserter.hand_size and inserter.hand_size > 1) and get_icon_path("long-handed") or "")
	local item = data.raw.item[inserter.name]
	if item ~= nil and icon ~= "" then
		if not item.icons then
			item.icons = {{icon = item.icon, icon_size = item.icon_size}}
		end
		table.insert(item.icons,{icon = icon, icon_size = DIR.icon_size, scale = 0.25, shift = {-8,8}})
		inserter.icons = item.icons
	end
end

-------------------------------------------------------------------------------------
