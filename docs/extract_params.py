import os
import json
import re

from pathlib import Path


COMPONENT_DIRECTORIES = [
    "../plugin/Hydro/",
    "../plugin/Mapping/",
    "../plugin/Plants/",
    "../plugin/Terrain/"
]

OUTPUT_DIRECTORY = "../site/_data/components/"


def parse_name(contents):
    lines = contents.splitlines()
    base_line = [s.strip() for s in lines if "base(" in s][0]
    args = base_line.split('"')[1::2]
    return {
        "name": args[0],
        "nickname": args[1],
        "description": args[2],
        "category": args[3],
        "subcategory": args[4]
    }


def parse_args(line):
    args = line.split('"')[1::2] # Split into substrings bounded by quotes
    return {
        "name": args[0],
        "id": args[1],
        "description": args[2],
        "optional": False,
        "type": re.search('Add(.*)Parameter', line).group(1),
    }


def parse_params(contents, type):

    if type == "input":
        function_to_find = "RegisterInputParams"
    else:
        function_to_find = "RegisterOutputParams"

    index_of_function = contents.find(function_to_find, 0)
    index_of_start = contents.find('{', index_of_function) + 1
    index_of_end = contents.find('}', index_of_function) - 1

    parameters = []
    lines = contents[index_of_start:index_of_end].splitlines()
    for line in lines:
        if "pManager" in line:
            if line.strip()[0:1] == "//":
                continue # Commented out lines
            elif "Add" in line:
                parameters.append(parse_args(line))
            elif ".Optional" in line and "true" in line:
                parameters[-1]["Optional"] = True # Set last added param optional

    return parameters


def parse_file(name, contents):
    # Write out file
    data = parse_name(contents)
    data["inputs"] = parse_params(contents, "input")
    data["outputs"] = parse_params(contents, "output")
    file_name = OUTPUT_DIRECTORY + name + ".json"
    with open(file_name, 'w') as outfile:
        json.dump(data, outfile, ensure_ascii=False, indent=2)


for directory in COMPONENT_DIRECTORIES:
    for file in os.listdir(directory):
        if file.endswith(".cs") and "Component" in file:
            dir = os.path.abspath(file)
            contents = Path(directory + file).read_text()
            component_name = file.replace("groundhog", "").replace("Component", "").replace(".cs", "")
            parse_file(component_name, contents)