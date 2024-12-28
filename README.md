# Unity-GameplayTags

My attempt to replicate Unreal's Gameplay Tags system: https://dev.epicgames.com/documentation/en-us/unreal-engine/gameplay-tags?application_version=4.27


# Usage

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

