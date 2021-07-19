# Contributing

## Getting started
Start by cloning the project and install dependencies and run the unit tests to make sure everything is working as expected.

## Versioning
Versioning for this project follows [SemVer](https://semver.org/) and it is enforced through CI/CD script run in Github Actions. [semantic-release](https://github.com/semantic-release/semantic-release) is used to analyze commit-messages and determine the next version. The commit messages must follow [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/#summary) pattern. See a quick go-through [below](#conventional-commits).

## Commiting
Not every commit message have to follow the pattern, sometimes it is better not to follow the pattern. However, the commits which should be used to generate and change-log and determine the next version must follow the pattern.


## Create pull request


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
