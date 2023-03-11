<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a name="readme-top"></a>

<!-- PROJECT LOGO -->
<br />
<div align="center">

[![LinkedIn][linkedin-shield]][linkedin-url]

<h3 align="center">NoFluffDI</h3>
  <p align="center">
    A no reflection dependency injection (technically only resolution) framework for Unity
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

NoFluffDI was created after hearing about and experiencing performance loss when using common DI framework, in my case Zenject; expecially on big, performance-critical projects like mobile games.

My first inspirations were reading about the term <a href="https://blog.ploeh.dk/2014/06/10/pure-di/">Pure-DI</a> and <a href="https://github.com/gustavopsantos/Reflex">Reflex</a>, another DI framework which also prioritized performance improvements and inspired many of my designs and after using it a bit I was convienced simpler more performant frameworks then Zenject can be created.

The main design pillar of NoFluffDI, differentiating it from the other frameworks is not using reflection at all.

This allowed me to kill 3 birds with 1 stone:

### - Gain performance boost
Reflection is known to be slow in general and in order to automatically inject dependencies in the scene, it's required to iterate and reflect on every component in the scene after every scene load, whether it has dependencies requiring injection or not.

My system instead inverts the flow of dependency resolution; instead of the framework finding components in need of injection, the container is bound to places in the scene hierarchy and the components look for the most relevant container to them, no reflection involved.

### - Easier to debug systems
Reflection makes it harder to debug errors because the compiler doesn't know where a type is used, where a instance came from and what resolver was used to create it.

Removing reflection means all types are still refrenced in the compiler, you can follow the stack to find a origin of an instance and the systems of the framework itself usually looks easier to understand with less "magic" methods.

### - More stable and easier to maintain
Reflection is also <a href="https://github.com/dotnet/runtime/blob/main/src/coreclr/nativeaot/docs/reflection-in-aot-mode.md">not very friendly with AOT compilation</a> (unlike Unity's most popular scripting backend - <a href=https://docs.unity3d.com/Manual/ScriptingRestrictions.html>IL2CPP</a>) or code stripping (also <a href=https://docs.unity3d.com/Manual/ManagedCodeStripping.html>very popular in Unity</a>), requiring you to follow and understand the risks of using a DI frameworks, making it riskier and more complex to use then it should be.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

* Unity 2021+

### Installation

#### UPM

Simpliest way is to use Unity's package manager:

Window -> Package Manager -> Add -> Add package from git url... -> https://github.com/NotFluffyGames/NoFluffDI.git

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage

Todo

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTACT -->
## Contact

Alon.Talmi @ Alon.Talmi@gmail.com

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/alon-talmi/
