float temporalLerp(float a, float b, float t, float delta) {
	return lerp(a, b, 1.0 - pow(1.0 - t, delta * 60.0));
}

float floatDist(float a, float b) {
    return abs(a - b);
}
