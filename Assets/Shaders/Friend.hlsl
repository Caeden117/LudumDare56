struct Friend {
    float2 position;
    float2 velocity;
    // Code for if we had HLSL 2021 support: https://devblogs.microsoft.com/directx/announcing-hlsl-2021/#bitfield-members-in-data-types
    /*uint state : 4;
    uint mood : 8;
    uint colorIdx : 8;
    uint reserved : 12;*/
    uint packed0;
    float fvar0;
    float fvar1;
    float fvar2;
    float fvar3;
};

#define FRIEND_GET_STATE(this) (this.packed0 & 0xFu)
#define FRIEND_GET_MOOD(this) ((this.packed0 >> 4) & 0xFFu)
#define FRIEND_GET_COLOR_IDX(this) ((this.packed0 >> 12) & 0xFFu)

#define FRIEND_SET_STATE(this, value) this.packed0 = (this.packed0 & ~0xFu) | ((value) & 0xFu)
#define FRIEND_SET_MOOD(this, value) this.packed0 = (this.packed0 & ~(0xFFu << 4)) | (((value) & 0xFFu) << 4)
#define FRIEND_SET_COLOR_IDX(this, value) this.packed0 = (this.packed0 & ~(0xFFu << 12)) | (((value) & 0xFFu) << 12)

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
