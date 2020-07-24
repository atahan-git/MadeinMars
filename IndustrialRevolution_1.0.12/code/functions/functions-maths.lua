------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-log")

------------------------------------------------------------------------------------------------------------------------------------------------------

function table_length(t)
    if not t then return nil end
    local count = 0
    for _ in pairs(t) do count = count + 1 end
    return count
end


function linear_transform(x,a,b,c,d)
	return (x-a)/(b-a)*(d-c)+c
end

function multiply_vector(v,f)
    return { v[1]*f, v[2]*f }
end


function get_vector_sum(v1,v2)
    if not v1 or not v2 then return nil end
    return { x = v1.x + v2.x, y = v1.y + v2.y}
end

function get_vector_difference(v1,v2)
    if not v1 or not v2 then return nil end
    return { x = v1.x - v2.x, y = v1.y - v2.y}
end

function list_contains(list,target)
    if not list then return false end
    for _,v in pairs(list) do
        if v == target then return true end
    end
    return false
end

function append_list(t1, t2)
    if not t1 then t1 = {} end
    if not t2 then t2 = {} end
    for k,v in pairs(t2) do
        table.insert(t1,v)
    end
    return t1
end

function values_to_keys(t)
    if not t then return nil end
    local t2 = {}
    for _,v in pairs(t) do t2[v] = true end
    return t2
end

function keys_to_values(t)
    if not t then return nil end
    local t2 = {}
    for k,_ in pairs(t) do table.insert(t2,k) end
    return t2
end

function get_keys_matching_value_list(t,l)
    if not t then return nil end
    local t2 = {}
    for k,v in pairs(t) do
        for _,v2 in pairs(l) do
            if k == v2 then
                table.insert(t2, v2)
            end
        end
    end
    return t2
end

function merge_keys(t1,t2)
    if not t2 then return t1 end
    if not t1 then return t2 end
    for k,v in pairs(t2) do
        t1[k] = v
    end
    return t1
end

function serialise(t)
    if t == nil then return nil end
    return table.concat(t, ", ")
end

function is_value_in_table(t, value)
    if not t or not value then return false end
    for k,v in pairs(t) do
        if value == v then return true end
    end
    return false
end

function clamp(val, mini, maxi)
    return math.min(math.max(val, mini), maxi)
end

function multiply_colour(colour, factor)
    if not colour.r then colour.r = colour[1] end
    if not colour.g then colour.g = colour[2] end
    if not colour.b then colour.b = colour[3] end
    if not colour.a then colour.a = colour[4] end
    return {r=colour.r*factor, g=colour.g*factor, b=colour.b*factor, a=colour.a}
end

-- brighter version of tier colour for working vis glow & lights
function brighter_colour(c)
    return { r = (c.r + 1)/2, g = (c.g + 1)/2, b = (c.b + 1)/2, a = c.a }
end

function seconds_to_ticks(s)
    return math.floor(s * 60)
end

-- expects (hue, saturation, value, alpha) - all between 0-1 - as input
-- returns a Factorio colour table
function hsva2rgba(h, s, v, a)
    local r, g, b
    local i = math.floor(h * 6);
    local f = h * 6 - i;
    local p = v * (1 - s);
    local q = v * (1 - f * s);
    local t = v * (1 - (1 - f) * s);
    i = i % 6
    if i == 0 then r, g, b = v, t, p
    elseif i == 1 then r, g, b = q, v, p
    elseif i == 2 then r, g, b = p, v, t
    elseif i == 3 then r, g, b = p, q, v
    elseif i == 4 then r, g, b = t, p, v
    elseif i == 5 then r, g, b = v, p, q
    end
    return { r = r, g = g, b = b, a = a }
end

function get_saturation(hue,saturation)
	if saturation == nil then return 1 end
	if hue > 0 then return saturation end
	return 0
end

function get_crafting_tints_from_hue(hue,saturation)
	return {
		primary = hsva2rgba(hue, get_saturation(hue,saturation), 1, 1),
		secondary = hsva2rgba(hue, get_saturation(hue,saturation)/2, 1, 1),
		tertiary = hsva2rgba(hue, get_saturation(hue,saturation), 0.75, 1),	
		quaternary = hsva2rgba(hue, get_saturation(hue,saturation)/2, 0.75, 1),	
	}
end

function format_number_to_units(amount,padding)
	if padding == nil then padding = true end
    local suffix = ""
    local suffix_list =
        {
            ["T"] = 1000000000000,
            ["G"] = 1000000000,
            ["M"] = 1000000,
            ["k"] = 1000
        }
    for letter, limit in pairs (suffix_list) do
        if math.abs(amount) >= limit then
            amount = math.floor(amount/(limit/10))/10
            suffix = letter
            break
        end
    end
    local formatted, k = amount
    while true do
        formatted, k = string.gsub(formatted, "^(-?%d+)(%d%d%d)", '%1,%2')
        if (k==0) then
            break
        end
    end	
    return formatted..((amount < 100 and math.abs(amount) == amount) and ".0" or "")..(padding and " " or "")..suffix
end

function get_area_of_box(box)
	return math.abs(box[1][1]-box[2][1]) * math.abs(box[1][2]-box[2][2])
end

------------------------------------------------------------------------------------------------------------------------------------------------------
