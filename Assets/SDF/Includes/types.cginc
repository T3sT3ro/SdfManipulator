typedef int ID;

struct Hit
{
    float distance;
    ID id;
};

struct RayInfo3D
{
    float3 ro;
    float3 rd;
    float3 p; /// 3D point in space where ray is evaluated
    int steps; /// how many steps did raymarcher do
    Hit hit; /// ray hit results
};
