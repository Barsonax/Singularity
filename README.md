# Singularity
Singularity is a ioc container that focuses on the following things
- Very high performance. Once the container is configured injecting dependencies is done by invoking a delegate. This means that performance in Singularity is measured in nanoseconds as opposed to microseconds in other containers. This makes it feasible to use singularity in applications where performance matters such as games. 
- No magic. Singularity has been designed in such a way that it won't hide too much from you. For instance `Dispose` wont be automagically called but instead you can configure Singularity to do so through the `OnDeath` method. This way you can always find out who is calling your methods.
- Clear error messages and fail fast to point you in the right direction as fast as possible.

Currently WIP
