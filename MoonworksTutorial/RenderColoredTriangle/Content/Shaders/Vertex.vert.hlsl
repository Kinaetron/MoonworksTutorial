﻿struct Input
{
    float2 Position : POSITION;
    float4 Color : COLOR0;
};

struct Output
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
};

Output main(Input input)
{
    Output output;
    output.Position = float4(input.Position, 0.0f, 1.0f);
    output.Color = input.Color;
    return output;
}