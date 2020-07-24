------------------------------------------------------------------------------------------------------------------------------------------------------

-- tweak vanilla signals

local symbols = {
	-- vanilla signals
	"A",
	"B",
	"C",
	"D",
	"E",
	"F",
	"G",
	"H",
	"I",
	"J",
	"K",
	"L",
	"M",
	"N",
	"O",
	"P",
	"Q",
	"R",
	"S",
	"T",
	"U",
	"V",
	"W",
	"X",
	"Y",
	"Z",
	"0",
	"1",
	"2",
	"3",
	"4",
	"5",
	"6",
	"7",
	"8",
	"9",
	"info",
	"each",
	"everything",
	"anything",
	"red",
	"green",
	"blue",
	"cyan",
	"yellow",
	"pink",
	"black",
	"white",
	"grey",
	-- DIR native signals
	"orange",
}

for _,symbol in pairs(symbols) do
	if data.raw["virtual-signal"]["signal-"..symbol] then
		data.raw["virtual-signal"]["signal-"..symbol].icon = get_icon_path(string.lower("signal_"..symbol),64)
		data.raw["virtual-signal"]["signal-"..symbol].icon_size = 64
	end
end

data.raw["virtual-signal"]["signal-check"].icon = get_icon_path("signal_check",64)
data.raw["virtual-signal"]["signal-check"].icon_size = 64
data.raw["virtual-signal"]["signal-check"].subgroup = "virtual-signal"
data.raw["virtual-signal"]["signal-check"].order = "b"

data.raw["virtual-signal"]["signal-dot"].icon = get_icon_path("signal_dot",64)
data.raw["virtual-signal"]["signal-dot"].icon_size = 64
data.raw["virtual-signal"]["signal-dot"].subgroup = "virtual-signal"
data.raw["virtual-signal"]["signal-dot"].order = "d"

------------------------------------------------------------------------------------------------------------------------------------------------------

-- new signals

data:extend({
	{
		name = "deadlock-signal-cross",
		icon = get_icon_path("signal_cross",64),
		icon_size = 64,
		order = "c",
		subgroup = "virtual-signal",
		type = "virtual-signal"
	},
	{
		name = "deadlock-signal-warning",
		icon = get_icon_path("signal_warning",64),
		icon_size = 64,
		order = "x",
		subgroup = "virtual-signal",
		type = "virtual-signal"
	},
	{
		name = "deadlock-signal-danger",
		icon = get_icon_path("signal_danger",64),
		icon_size = 64,
		order = "y",
		subgroup = "virtual-signal",
		type = "virtual-signal"
	},
	{
		name = "deadlock-signal-help",
		icon = get_icon_path("signal_help",64),
		icon_size = 64,
		order = "g",
		subgroup = "virtual-signal",
		type = "virtual-signal"
	},
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- rainbow!

if not mods["Dectorio"] then

	data:extend({{
		name = "signal-orange",
		icon = get_icon_path("signal_orange",64),
		icon_size = 64,
		order = "d[colors]-[3yellow]",
		subgroup = "virtual-signal-color",
		type = "virtual-signal"
	}})

	local colours = {
		red = "a",
		orange = "b",
		yellow = "c",
		green = "d",
		cyan = "e",
		blue = "f",
		pink = "g",
		white = "h",
		grey = "i",
		black = "j",
	}

	for colour,order in pairs(colours) do
		if data.raw["virtual-signal"]["signal-"..colour] then
			data.raw["virtual-signal"]["signal-"..colour].order = "aardvark-"..order
		end
	end

end
------------------------------------------------------------------------------------------------------------------------------------------------------
