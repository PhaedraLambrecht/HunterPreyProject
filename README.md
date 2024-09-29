# Predator and prey

## Overview
This unity project was intended as a tiny research project on behaviour trees combined with AI characters that can interact with each other.
In this project, the character with the camera is the predator.


## Table of contents
1. AI Behaviors
   1.1. Predator
   1.2. Prey
2. Technical Implementation
3. Lessons Learned
4. Future Enhancements


## AI Behaviors
### Predator
The behaviour tree that the predator follows goes as follows:
1. **Wander**: The predator moves randomly across the terrain until it detects prey.
2. **Search**: If the prey is within a certain radius (smell radius) but not in view, it will search for it.
3. **Pursuit**: Once the predator sees the prey it will start the pursuit behaviour.

### Prey
The behaviour tree that the predator follows goes as follows:
1. **Wander**: The predator moves randomly across the terrain until it is too close to the predator.
2. **Evasion**: The prey attempts to flee and avoid capture if the predator is detected.


## Technical Implementation
### Lévy Flight Algorithm
I used Levy flight in my wander behaviour to create a more natural feeling of wandering around the world.
Levy flight in short means that the AI wanders in short steps with occasional longer steps.


> Unlike Brownian motion, there is a class of random walks called Lévy Flights, for which one cannot use the CLT. A Lévy Flight is composed of a series of small displacements, interspersed occasionally by a very large displacement.

**Source**: Barbosa, Barthelemy, et al. "Human mobility: Models and applications."

![LevyFlight](https://github.com/user-attachments/assets/37a40049-df60-4337-9111-5221d017e80f)

## Behavior Tree
A behavior tree is a decision-making structure, similar to a finite state machine. It is used to manage AI behaviours by breaking them down into smaller, more manageable tasks.


The video I followed, [Behavior Trees in Unity](https://youtu.be/KeShMInMjro?si=c-kyLmQC8a6NTsvO), explains how behaviour trees function. by showing the difference between a fallback and a sequence.
-> Where a fallback is an or statement having either this or that happen. For example, eating a sandwich or eating an apple.
-> While a sequence is an and statement having both actions happen once the previous one is successful. For example, peeling a banana and then eating that banana.

