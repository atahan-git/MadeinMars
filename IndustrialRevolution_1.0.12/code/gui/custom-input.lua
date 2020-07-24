------------------------------------------------------------------------------------------------------------------------------------------------------

-- adjustable inserters

if settings.startup["deadlock-industry-inserter-adjustment"].value then
    data:extend({
        {
            type = "custom-input",
            name = "deadlock-inserter-dropoff",
            key_sequence = "SHIFT + E",
            consuming = "game-only",
        },
    })
end

-- turret adjustment

data:extend({
    {
        type = "custom-input",
        name = "deadlock-turret-angle-toggle",
        key_sequence = "SHIFT + E",
        consuming = "game-only",
    },
})

-- SEWF mock gui

data:extend({
    {
        type = "custom-input",
        name = "deadlock-open-gui",
        key_sequence = "",
        linked_game_control = "open-gui",
    },
})

-- change blueprinting unlocks to relevant tech

for _,shortcut in pairs(data.raw.shortcut) do
    if shortcut.technology_to_unlock == "construction-robotics" then shortcut.technology_to_unlock = "personal-roboport-equipment" end
end

-- burner leech shortcut

data:extend({
    {
        type = "custom-input",
        name = "deadlock-control-generator-manager",
        key_sequence = "CONTROL + G",
    },
    {
        name = "deadlock-industry-generator-manager",
        type = "shortcut",
        toggleable = true,
        action = "lua",
        associated_control_input = "deadlock-control-generator-manager",
        technology_to_unlock = "heavy-armor",
        icon = {
            filename = get_icon_path("generator-manager-black-32",32),
            priority = "extra-high-no-scale",
            size = 32,
            scale = 1,
            flags = {"icon"}
        },
        disabled_icon = {
            filename = get_icon_path("generator-manager-white-32",32),
            priority = "extra-high-no-scale",
            size = 32,
            scale = 1,
            flags = {"icon"}
        },
        small_icon = {
            filename = get_icon_path("generator-manager-black-24",24),
            priority = "extra-high-no-scale",
            size = 24,
            scale = 1,
            flags = {"icon"}
        },
        disabled_small_icon = {
            filename = get_icon_path("generator-manager-white-24",24),
            priority = "extra-high-no-scale",
            size = 24,
            scale = 1,
            flags = {"icon"}
        },
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

