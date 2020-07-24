------------------------------------------------------------------------------------------------------------------------------------------------------

-- startup settings

data:extend({
    {
        type = "string-setting",
        name = "deadlock-industry-laser-colour",
		order = "a",
        setting_type = "startup",
        default_value = "red",
		allowed_values = {"red","orange","yellow","lime","green","teal","cyan","azure","blue","violet","magenta","pink"}
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-hide-redundancies",
		order = "b",
        setting_type = "startup",
        default_value = true,
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-inserter-adjustment",
		order = "c",
        setting_type = "startup",
        default_value = true,
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-enable-burner-leech",
		order = "d",
        setting_type = "startup",
        default_value = false,
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-building-scrap",
		order = "e",
        setting_type = "startup",
        default_value = false,
    },
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- map settings

data:extend({
    {
        type = "string-setting",
        name = "deadlock-industry-starting-age",
		order = "a",
        setting_type = "runtime-global",
        default_value = "stone-age",
		allowed_values = {"stone-age","bronze-age","iron-age","steel-age","chrome-age","future-age"}
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-starting-kit",
		order = "b",
        setting_type = "runtime-global",
        default_value = false,
    },
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- per player settings

data:extend({
    {
        type = "string-setting",
        name = "deadlock-industry-tip-frequency",
		order = "a",
        setting_type = "runtime-per-user",
        default_value = "0",
		allowed_values = {"10","30","60","0"}
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-research-notifications",
		order = "b",
        setting_type = "runtime-per-user",
        default_value = false,
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-genman-pingemptyslot",
		order = "c",
        setting_type = "runtime-per-user",
        default_value = true,
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-genman-pingnofuel",
		order = "d",
        setting_type = "runtime-per-user",
        default_value = true,
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-genman-pingnoinv",
		order = "e",
        setting_type = "runtime-per-user",
        default_value = true,
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-genman-inventorysounds",
		order = "f",
        setting_type = "runtime-per-user",
        default_value = true,
    },
    {
        type = "bool-setting",
        name = "deadlock-industry-genman-pingsounds",
		order = "g",
        setting_type = "runtime-per-user",
        default_value = true,
    },
})

------------------------------------------------------------------------------------------------------------------------------------------------------
