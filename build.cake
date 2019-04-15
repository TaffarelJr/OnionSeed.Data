#load "./build/DotNet.cake"

var target = Argument("target", "CI");

RunTarget(target);
