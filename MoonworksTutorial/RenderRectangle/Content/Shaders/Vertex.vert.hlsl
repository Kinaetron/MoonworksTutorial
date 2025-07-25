struct Input
{
    float2 Position : POSITION;
};

struct Output
{
    float4 Position : SV_Position;
};

Output main(Input input)
{
    Output output;
    output.Position = float4(input.Position, 0.0f, 1.0f);
    return output;
}