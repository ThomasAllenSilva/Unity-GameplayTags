# Unity-GameplayTags

My attempt to replicate Unreal's Gameplay Tags system: https://dev.epicgames.com/documentation/en-us/unreal-engine/gameplay-tags?application_version=4.27


# Build
Before building your project make sure that you added your tags container as a preloaded asset, otherwise it will get stripped because there's nothing directly referencing it


<img width="738" alt="11" src="https://github.com/user-attachments/assets/2c02b2b8-d8cd-4a6d-a48e-3a8f6a3aa260" />


# Registering tags
You can register tags in two ways: Adding native tag / Manually adding a new tag

# Manually adding a new tag
You can manipulate tags directly on the ScriptableObject the following way:

1 - Click "Add Root Tag"


2 - Edit Tag Name


3 - Add Subtag


4 - Delete Root/Subtag


<img width="497" alt="0" src="https://github.com/user-attachments/assets/5a1f8dd9-8284-4d81-812d-f52437b36c4d" />

# Adding native tag

To add a native tag you must do that via code the following way:

1 - Locate the NativeGameplayTags class or the one that you created


2 - Add a new const string with the tag name (this const string can be used anywhere in your code to retrieve the tag and avoid typos)

<img width="491" alt="4" src="https://github.com/user-attachments/assets/a98e4280-9b89-4857-9187-d15e3f0856dc" />

3 - Use the AddNativeTag method from the container to add a new native tag

<img width="370" alt="5" src="https://github.com/user-attachments/assets/9af73122-8274-4014-b8d7-7a6e0e060107" />

# Native tags notes

1 - Native tags cannot be deleted on modified directly on the ScriptableObject, they're readonly

2 - If you add a native tag that already exists inside the container, it will replace the tag by the native one

3 - If you remove a native tag from code, it will not revert as a manual tag, it will just be removed from the container

4 - Native tags can also have child tags both added from the ScriptableObject and from code

# Gameplay Tags Component
Use this component to hold tags related to your GameObjects

<img width="494" alt="1" src="https://github.com/user-attachments/assets/263728c2-10f6-4651-9517-925a4f8a370b" />

You can add Startup Tags and also Add/Remove tags during Runtime

# Addings Tag To Gameplay Tags Component

On the Startup Tags you can select any tag from the created ones inside the ScriptableObject (Including native tags) and add as many as you want

<img width="186" alt="2" src="https://github.com/user-attachments/assets/63e3715c-f92d-4e83-81d1-9c9f85f29691" />

You can also add via code during runtime which will then trigger an event so that you can perform actions whenever a tag is added/removed

<img width="508" alt="6" src="https://github.com/user-attachments/assets/d40cf1a5-c161-412a-a4d7-b918f87b03a7" />


# GameplayTagSelector Attribute

If you want to select a tag from your custom component you can use the GameplayTagSelector Attribute which allows you to choose a tag to your string field and avoid typos

<img width="318" alt="7" src="https://github.com/user-attachments/assets/58427a37-db5d-4de9-8de5-76613d208569" />


<img width="498" alt="8" src="https://github.com/user-attachments/assets/d9f4e445-83cd-4e16-9df9-d6acba2f537e" />



# Extension Methods

There's also an extension methods helper class which helps you retrieve/add/remove tags from Objects

<img width="491" alt="9" src="https://github.com/user-attachments/assets/97a5e42e-439f-40d2-a5f4-827a1d77fd02" />


<img width="400" alt="10" src="https://github.com/user-attachments/assets/517ee205-a454-4843-ade3-5c6f495215db" />


# Future features

[ ] Replace strings by Guids to improve speed and memory usage - I'm still analysing how I'm going to implement this because there's also the possibility of using string interning, which may improve memory usage for the manually added tags inside the ScriptableObject

[ ] Add more tags comparison fuctions inside Gameplay Tags Component
