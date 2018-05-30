# Singularity
Singularity is a ioc container that focuses on the following things
- Very high performance. Once the container is configured injecting dependencies is done by a dictionary lookup and simply invoking a delegate. This means that performance in Singularity is measured in nanoseconds as opposed to microseconds in other containers. This makes it feasible to use singularity in applications where performance matters such as games. 
- No magic. Singularity has been designed in such a way that it won't hide too much from you. For instance `Dispose` wont be automagically called but instead you can configure Singularity to do so through the `OnDeath` method. This way you can always find out who is calling your methods.
- Clear error messages and fail fast to point you in the right direction as fast as possible.

# Installation
`Singularity` can be installed through nuget. There is also support for the [Duality](https://duality.adamslair.net/) game engine through the `Singularity.Duality` nuget package

# Documentation
More info about `Singularity` can be found on the wiki. I suggest you to start [here](https://github.com/Barsonax/Singularity/wiki/Configuring-Dependencies). For duality users there is also a guide on how to use `Singularity` in duality [here](https://github.com/Barsonax/Singularity/wiki/Using-Singularity-in-Duality).
