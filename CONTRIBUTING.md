# Contributing

## Getting started
Start by cloning the project and install dependencies and run the unit tests to make sure everything is working as expected.

## Versioning
Versioning for this project follows [SemVer](https://semver.org/) and it is enforced through CI/CD script run in Github Actions. [semantic-release](https://github.com/semantic-release/semantic-release) is used to analyze commit-messages and determine the next version. The commit messages must follow [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/#summary) pattern. See a quick go-through [below](#conventional-commits).

### Apply the format

Not every commit message has to follow the pattern, only the one which will be used to determine the verison and generate the change-log. You can have several commit messages in a single pull-request with different tags and scopes and you can forget about it until you merge a pull-request. If you use the latter, you have to remember to add the tag and description when merging the pull request, in order to generate a new release.

## Branches
CI/CD is set-up to support relase and pre-release. The release is created when a pull-request agianst the main-branch (main/master) is completed and a pre-relase (dev suffix) is created when a pull-request against the dev-branch is completed. 

Furthermore, `todoes` in code is automatically created as issues and the issues are closed when the `todo` is removed. See [this](https://github.com/marketplace/actions/todo-to-issue#todo-options) for more details.

### Branch-protection
Branch-protection is enabled.

* You cannot push to the main-branch
* Build and test actions have to be succesfull before a pull-request can be merged.


# Conventional Commits

Commit messages following the pattern will be used to determine the next version and to generate a change-log. The pattern is:

```
<tag>(<scope>): <short description>

<elaborate-description>

<footer>
```

* `tag` can be: `feat | fix | doc | ci | `[*`and more`*](https://www.conventionalcommits.org/en/v1.0.0/#specification)
* `scope` is optional and are used to bundle changes togehter
* `short description` is a short description about the change
* `elaborate-description` is optional, but it is used to describe changes in more detail
* `footer` is optional, bu typically used to contain tags, s.a. `close #31`
