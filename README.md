# Tetris WinForms Game

A small **Tetris‑like game** built with **C#** and **Windows Forms**.

## Features

- **Classic 10×20 board**
- **7 tetromino types** (I, O, T, S, Z, J, L)
- **Keyboard controls**
  - Left / Right: move piece
  - Up: rotate
  - Down: soft drop
  - Space: hard drop (instant)
- **Line clearing and scoring**
- **Speed increases** as you clear lines
- **Next piece preview**
- **Textured pieces** for each tetromino color

## Requirements

- Windows with **.NET 8 SDK** or later
- **Visual Studio 2022** (or later) with:
  - **.NET desktop development** workload

## Getting Started

1. Open **Visual Studio**.
2. Choose **"Open a project or solution"**.
3. Select the file `TetrisWinForms.csproj` in this folder.
4. Set the project as **Startup Project** if needed.
5. Press **F5** to build and run.

## How It Works

- The main form is `GameForm`, created from `Program.cs`.
- Game state is stored in a 2D grid and updated on a timer.
- Rendering is done in the form's `Paint` event with double buffering.

## Controls Recap

- **← / →**: Move piece left / right  
- **↑**: Rotate piece  
- **↓**: Soft drop  
- **Space**: Hard drop  

When the board fills up and no new piece can spawn, the game shows **Game Over**; press **Space** to restart.

