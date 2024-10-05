float temporalLerp(float a, float b, float t, float delta) {
	return lerp(a, b, 1.0 - pow(1.0 - t, delta * 60.0));
}
