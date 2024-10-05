struct Friend {
    float2 position;
    float2 velocity;
    float3 color;
    uint state;
    float fvar0;
    float fvar1;
    float fvar2;
    float fvar3;
};

#define FRIEND_STATE_UNSET 0
#define FRIEND_STATE_WANDERING 1
#define FRIEND_STATE_WINDOW_SEEKING 2
#define FRIEND_STATE_WINDOW_RIDING 3
#define FRIEND_STATE_HUNGRY 4

#define WANDERING_JUMP_TIMER(this) (this.fvar0)

#define WINDOW_SEEKING_DIRECTION(this) (this.fvar0)
#define WINDOW_SEEKING_JUMP_DISTANCE(this) (this.fvar1)
#define WINDOW_SEEKING_CANCEL_TIMER(this) (this.fvar2)

#define WINDOW_RIDING_POS_X(this) (this.fvar0)
#define WINDOW_RIDING_POS_Y(this) (this.fvar1)
#define WINDOW_RIDING_DIRECTION(this) (this.fvar2)
#define WINDOW_RIDING_JUMP_TIMER(this) (this.fvar3)
