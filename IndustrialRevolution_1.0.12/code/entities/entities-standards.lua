------------------------------------------------------------------------------------------------------------------------------------------------------

function standard_3x3_collision()
    return { {-1.25,-1.25}, {1.25,1.25} }
end

function standard_3x3_selection()
    return { {-1.5,-1.5}, {1.5,1.5} }
end

function standard_entity_info_icon_shift()
    return {0,-0.625}
end

function standard_grinder_info_icon_shift()
    return {0,-0.45}
end

function standard_entity_info_module_shift()
    return {0,0.3}
end

function standard_2x2_turret_info_icon_shift()
    return {0,0.3}
end

function standard_3x3_turret_info_icon_shift()
    return {0,0.55}
end

function standard_small_chest_collision()
    return data.raw.container["wooden-chest"].collision_box
end

function standard_pipe_pictures()
    return {
        north = get_layer("machines/standard-pipe-pictures", nil, nil, false, nil, nil, 52, 31, 94, 4, 238, 238, {0,2}),
        east = get_layer("machines/standard-pipe-pictures", nil, nil, false, nil, nil, 30, 82, 193, 82, 238, 238, {-2,0}),
        south = get_layer("machines/standard-pipe-pictures", nil, nil, false, nil, nil, 70, 64, 88, 151, 238, 238, {0,-2}),
        west = get_layer("machines/standard-pipe-pictures", nil, nil, false, nil, nil, 30, 82, 15, 82, 238, 238, {2,0}),
    }
end

function standard_fluid_boxes()
    return {
        {
            production_type = "input",
            pipe_picture = standard_pipe_pictures(),
            pipe_covers = pipecoverspictures(),
            base_area = 10,
            base_level = -1,
            pipe_connections = {{ type="input", position = {0, -2} }},
            secondary_draw_orders = { north = -1 }
        },
        {
            production_type = "output",
            pipe_picture = standard_pipe_pictures(),
            pipe_covers = pipecoverspictures(),
            base_area = 10,
            base_level = 1,
            pipe_connections = {{ type="output", position = {0, 2} }},
            secondary_draw_orders = { north = -1 }
        },
        off_when_no_fluid_recipe = true
    }
end

------------------------------------------------------------------------------------------------------------------------------------------------------
